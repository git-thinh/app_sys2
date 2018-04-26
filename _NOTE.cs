using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace app_sys
{
    // You can use ThreadPool.RegisterWaitForSingleObject to wait for your own done event for each thread.  
    // The thread pool is smart enough to make use of a native thread's ability to block on multiple wait handles 
    // (up to 64 per thread, I believe).  So, theoretically, 
    // it will only create one additional thread per 64 threads you are waiting on. 
    // Here's an example of making use of this: 
    class test
    {
        static void TEST(string[] args)
        {
            var allDone = new ManualResetEvent(false);
            const int numberOfThreadToCreate = 20;

            int threadsRemainingToFinish = numberOfThreadToCreate;
            object sync = new object();
            for (int i = 0; i < 20; i++)
            {
                var doneEvent = new ManualResetEvent(false);
                Thread thread = new Thread(new ParameterizedThreadStart(delegate (object evt)
                {
                    Thread.Sleep(200); // simulate lengthy operation
                    ((ManualResetEvent)evt).Set();
                }));
                thread.Start(doneEvent);
                ThreadPool.RegisterWaitForSingleObject(doneEvent, new WaitOrTimerCallback(delegate (object index, bool timedOut)
                {
                    Console.WriteLine(index);
                    lock (sync)
                    {
                        if (--threadsRemainingToFinish == 0)
                        {
                            allDone.Set();
                        }
                    }
                }), i, -1, true);
            }

            allDone.WaitOne();
        }
    }
}
