using System;
using System.Collections.Generic;
using System.Text;

namespace app_sys
{
    public class vToolbar : SubscriberMsg
    {
        public vToolbar(Publisher<MSG> publisher) : base(publisher)
        {
        }
    }
}
