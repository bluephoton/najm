using System;
using System.Collections.Generic;
using System.Text;

namespace Najm.Diag
{
    class PerfTest
    {
        internal static void Start()
        {
            _pri = System.Threading.Thread.CurrentThread.Priority;
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
            _before = Environment.TickCount;
        }

        internal static void Stop()
        {
            Stop(null);
        }

        internal static void Stop(string lable)
        {
            _duration = Environment.TickCount - _before;
            System.Threading.Thread.CurrentThread.Priority = _pri;
            if (!string.IsNullOrEmpty(lable))
            {
                Report(lable);
            }
        }

        internal static void Report(string lable)
        {
            System.Windows.Forms.MessageBox.Show(lable + _duration.ToString());
        }

        private static long _before;
        private static long _duration;
        private static System.Threading.ThreadPriority _pri;
    }
}
