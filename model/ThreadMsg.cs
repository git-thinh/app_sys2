using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace app_sys
{
    public class ThreadMsgPara
    {
        private readonly Publisher<MSG> _publisher;
        private readonly ManualResetEvent _resetEvent;
        public ThreadMsgPara(Publisher<MSG> publisher, ManualResetEvent resetEvent)
        {
            _publisher = publisher;
            _resetEvent = resetEvent;
        }

        public Publisher<MSG> Publisher { get { return _publisher; } }
        public ManualResetEvent ResetEvent { get { return _resetEvent; } }
    }

    public class ThreadMsg
    {
        private readonly Thread _threadCrawler;
        private readonly ManualResetEvent _threadEvent;
        private readonly ManualResetEvent _resetEvent;
        private readonly Action<MSG> _action;
        private MSG _msg;
        private bool _exit = false;

        public ThreadMsg(Action<MSG> action)
        {
            _action = action;
            _resetEvent = new ManualResetEvent(false);
            _threadEvent = new ManualResetEvent(false);
            _threadCrawler = new Thread(new ParameterizedThreadStart(delegate (object evt)
            {
                ThreadMsgPara tm = (ThreadMsgPara)evt;
                while (_exit == false)
                {
                    action(_msg);
                    tm.ResetEvent.WaitOne();
                }
            }));
            _threadCrawler.Start(new ThreadMsgPara(null, _resetEvent));
        }

        public void Execute(MSG msg)
        {
            _msg = msg;
            _resetEvent.Set();
        }

        public void Stop()
        {
            _exit = true;
            _resetEvent.Set();
        }
    }
}
