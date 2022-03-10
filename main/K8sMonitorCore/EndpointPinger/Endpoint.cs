using System;

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

        public void Fail() {
            fails++;
            if (fails >= FailureThreshold + 1) {
                Status = StatusType.Dead;
                fails = FailureThreshold + 1;
            } else {                
                Status = StatusType.Dying;
            }
        }

        public void Success() {
            fails--;
            if (fails <= 0) {
                Status = StatusType.Healthy;
                fails = 0;
            }
            else {                
                Status = StatusType.Recovering;
            }
        }

    }
}
