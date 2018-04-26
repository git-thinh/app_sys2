using System;
using System.Collections.Generic;
using System.Text;

namespace app_sys
{
    public class vListItem : SubscriberMsg
    {
        public vListItem(Publisher<MSG> publisher) : base(publisher)
        {
        }
    }
}
