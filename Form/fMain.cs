using System;
using System.Collections.Generic;
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
                    MessageBox.Show(mystr.lpData);
                    break;
            }
            base.WndProc(ref m);
        }

        public fMain()
        {
        }


    }
}
