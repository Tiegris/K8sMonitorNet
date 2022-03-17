using System;
using System.Threading;

namespace Pinger
{
    public class Endpoint
    {
        public StatusType Status { get; private set; }
        public int FailureThreshold { get; init; }
        public TimeSpan Timeout { get; init; }
        public TimeSpan Period { get; init; }
        public Uri Uri { get; init; }

        private int fails = 0;

        internal void Fail() {
            var temp = Interlocked.Increment(ref fails);
            if (temp >= FailureThreshold + 1) {
                Status = StatusType.Dead;
                Interlocked.Exchange(ref fails, FailureThreshold + 1);
            }
            else {
                Status = StatusType.Dying;
            }
        }

        internal void Success() {
            Interlocked.Decrement(ref fails);
            if (fails <= 0) {
                Status = StatusType.Healthy;
                Interlocked.Exchange(ref fails, 0);
            }
            else {
                Status = StatusType.Recovering;
            }
        }

    }
}
