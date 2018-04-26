using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace app_sys
{
    //Used for WM_COPYDATA for string messages
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }

    public class MsgOutput
    {
        public bool Ok { set; get; } = false;
        public int Total { set; get; } = 0;
        public object Data { set; get; } 
    }

    public class MSG
    {
        public MSG_COMMAND Command { set; get; }
        public string Key { set; get; } = string.Empty;
        public object Input { set; get; }
        public MsgOutput Output { set; get; } 

        public static MSG create_Key(string key, object data)
        {
            return new MSG() { Command = MSG_COMMAND.KEY_CUSTOM, Input = data, Key = key };
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
