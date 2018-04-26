using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace app_sys
{ 
    public class MSG
    {
        public MSG_COMMAND Command { set; get; }
        public string Key { set; get; } = string.Empty;
        public object Data { set; get; }

        public static MSG create_Key(string key, object data) {
            return new MSG() { Command = MSG_COMMAND.KEY_CUSTOM, Data = data, Key = key };
        }
    }

    public class SubscriberMsg
    {
        private readonly Publisher<MSG> _publisher;

        public SubscriberMsg(Publisher<MSG> publisher)
        {
            _publisher = publisher;
        }

        public void Subscribe(Action<GenericEventArgs<MSG>> onMessage)
        {
            _publisher.OnMessage += (sender, msg) => onMessage(msg);
        }
    }

}
