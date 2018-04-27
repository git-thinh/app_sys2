using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace app_sys
{
    public class fMain : Form
    {
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MessageHelper.WM_USER:
                    MessageBox.Show("Message recieved: " + m.WParam + " - " + m.LParam);
                    break;
                case MessageHelper.WM_COPYDATA:
                    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
                    string func = mystr.lpData;

                    MessageBox.Show(func);
                    Type thisType = this.GetType();
                    MethodInfo method = thisType.GetMethod(func);
                    object[] para = null;
                    if (method != null) method.Invoke(this, para);
                    break;
            }
            base.WndProc(ref m);
        }

        public fMain()
        {
        }


    }
}
