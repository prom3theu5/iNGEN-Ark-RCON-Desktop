using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PTK.Utils
{
    public static class Repeat
    {
        public static Task Interval(
            TimeSpan pollInterval,
            Action action,
            CancellationToken token,
            bool isRepeat)
        {
            return Task.Factory.StartNew(
                async () =>
                {
                    if (!isRepeat)
                    {
                        await Task.Delay(pollInterval.Milliseconds);
                        action();
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (token.WaitCancellationRequested(pollInterval)) 
                                break;
                            action();
                        }
                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }

    static class CancellationTokenExtensions
    {
        public static bool WaitCancellationRequested(
            this CancellationToken token,
            TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}
