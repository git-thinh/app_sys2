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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.ComponentModel;

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

        static Dictionary<string, IThreadMsg> dicService = null;
        static object lockResponse = new object();
        static Dictionary<string, MSG> dicResponses = null;

        static MessageHelper msgHelper;
        public static void postMessageToFormUI(string key)
        {
            int k = 0;
            k = msgHelper.sendWindowsStringMessage(0, key);
            //Or for an integer message
            //k = msgHelper.sendWindowsMessage(MessageHelper.WM_USER, 123, 456);
        }

        public static string postMessageToService(MSG m)
        {
            if (m == null) return string.Empty;
            if (m.Key == string.Empty)
                m.Key = Guid.NewGuid().ToString();
            switch (m.Command)
            {
                case MSG_COMMAND.TRANSLATE_GOOGLE_WORDS_REQUEST:
                    if (dicService.ContainsKey(_CONST.SERVICE_TRANSLATOR))
                        dicService[_CONST.SERVICE_TRANSLATOR].Execute(m);
                    break;
            }
            return m.Key;
        }

        public static void RUN()
        {
            dicResponses = new Dictionary<string, MSG>();
            dicService = new Dictionary<string, IThreadMsg>();
            fmain = new fMain();
            msgHelper = new MessageHelper() { Handle = (int)fmain.Handle };



            //||| CRAWLER


            //||| TRANSLATOR
            dicService.Add(_CONST.SERVICE_TRANSLATOR, new ThreadMsg((m) =>
            {
                if (m == null || m.Input == null) return;
                string[] a = (string[])m.Input;
                Encoding encoding = Encoding.UTF7;
                string input = string.Join("\r\n", a);
                //string temp = HttpUtility.UrlEncode(input.Replace(" ", "---"));
                string temp = HttpUtility.UrlEncode(input);
                //temp = temp.Replace("-----", "%20");

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
                    s = "<span " + s.Substring(p, s.Length - p);
                p = s.IndexOf("</div>");
                if (p > 0)
                {
                    s = s.Substring(0, p);
                    s = s.Replace("<br>", "¦");
                    s = HttpUtility.HtmlDecode(s);
                    result = Regex.Replace(s, @"<[^>]*>", String.Empty);
                }
                if (result != string.Empty)
                {
                    string[] rs = result.Split('¦').Select(x => x.Trim()).ToArray();
                    m.Output = new MsgOutput() { Ok = true, Data = rs, Total = rs.Length };
                    m.Key = input;
                    lock (lockResponse)
                    {
                        if (dicResponses.ContainsKey(m.Key))
                            dicResponses[m.Key] = m;
                        else
                            dicResponses.Add(m.Key, m);
                    }
                }
                else
                {
                    m.Output = new MsgOutput() { Data = "Can not translate" };
                }
                postMessageToFormUI(m.Key);
            }));

            //||| MAIN
            fmain.Shown += main_Shown;
            fmain.FormClosing += (se, ev) =>
            {
                foreach (var kv in dicService)
                    if (kv.Value != null)
                        kv.Value.Stop();
                // wait for complete threads
                Thread.Sleep(200);
            };

            postMessageToService(new MSG() { Command = MSG_COMMAND.TRANSLATE_GOOGLE_WORDS_REQUEST, Input = new string[] { "hello", "send message" } });

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
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            app.RUN();
        }
    }
}

