using System;
using System.Collections.Generic;
using System.Text;

namespace app_sys
{
    public class vContent : SubscriberMsg
    {
        public vContent(Publisher<MSG> publisher) : base(publisher)
        {
        }
    }
}
