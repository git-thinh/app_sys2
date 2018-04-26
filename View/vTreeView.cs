using System;
using System.Collections.Generic;
using System.Text;

namespace app_sys
{
    public class vTreeView : SubscriberMsg
    {
        public vTreeView(Publisher<MSG> publisher) : base(publisher)
        {
        }

    }
}
