using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace app_sys
{
    public class fMain:  Form
    {
        private readonly Publisher<MSG> _publisher; 
        public fMain(Publisher<MSG> publisher)
        {
            _publisher = publisher;
        }


    }
}
