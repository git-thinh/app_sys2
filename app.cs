using System;
using System.Security.Permissions;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using System.Net;
using System.Text;
using System.Linq;

namespace app_sys
{
    [PermissionSet(SecurityAction.LinkDemand, Name = "Everything"), PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class app
    {
        static app()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (se, ev) =>
            {
                Assembly asm = null;
                string comName = ev.Name.Split(',')[0];
                string resourceName = @"DLL\" + comName + ".dll";
                var assembly = Assembly.GetExecutingAssembly();
                resourceName = typeof(app).Namespace + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        byte[] buffer = new byte[stream.Length];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                                ms.Write(buffer, 0, read);
                            buffer = ms.ToArray();
                        }
                        asm = Assembly.Load(buffer);
                    }
                }
                return asm;
            };
        }

        static fMain fmain;
        static Publisher<MSG> publisher;

        static ThreadMsg threadCrawler;
        static ThreadMsg threadTranslater;

        public static void RUN()
        {
            publisher = new Publisher<MSG>();

            ///// CRAWLER
            

            ///// CRAWLER
            threadTranslater = new ThreadMsg((m) => {
                if (m == null || m.Data == null) return;
                string[] a = (string[])m.Data;
                a = a.Select(x => x.Replace("-----", "--").Replace("---", "--")).ToArray(); 
                Encoding encoding = Encoding.UTF7;
                string input = string.Join("-----", a);

                string temp = HttpUtility.UrlEncode(input.Replace(" ", "---"));
                temp = temp.Replace("-----", "%20");
                string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", temp, "en|vi");

                string s = String.Empty;
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = encoding;
                    s = webClient.DownloadString(url);
                }
                string ht = HttpUtility.HtmlDecode(s);

                string result = String.Empty;
                int p = s.IndexOf("id=result_box");
                if (p > 0)
                    s = s.Substring(p, s.Length - p);
                p = s.IndexOf("</span>");
                if (p > 0)
                {
                    s = s.Substring(0, p);
                    p = s.IndexOf(@"'"">");
                    if (p > 0)
                    {
                        result = s.Substring(p + 3, s.Length - (p + 3));
                        result = HttpUtility.HtmlDecode(result);
                    }
                }
                if (result != string.Empty)
                {
                    string[] rs = result.Split(new string[] { "-----" }, StringSplitOptions.None).Select(x => x.Replace("---", " ")).ToArray();

                } 
            });

            ///// MAIN
            fmain = new fMain(publisher);
            fmain.Shown += main_Shown;
            fmain.FormClosing += (se, ev) => {
                if (threadTranslater != null) threadTranslater.Stop();
                if(threadCrawler != null) threadCrawler.Stop();
                Thread.Sleep(200); // wait for complete thread crawler
            };

            publisher.Publish(new MSG() { Command = MSG_COMMAND.TRANSLATE_GOOGLE_WORDS_REQUEST, Data = new string[] { "hello", "send message" } });

            Application.Run(fmain);
        }

        private static void main_Shown(object sender, EventArgs e)
        {
            int wi = Screen.PrimaryScreen.WorkingArea.Width / 2;
            fmain.Left = wi;
            fmain.Width = wi;
            fmain.Top = 45;
            fmain.Height = Screen.PrimaryScreen.WorkingArea.Height - 45;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            app.RUN();
        }
    }
}

