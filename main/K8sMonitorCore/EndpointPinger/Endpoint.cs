using System;

namespace Pinger
{
    public delegate void EndpointStatusChanged(StatusType newStatus);

    public class Endpoint
    {

        public event EndpointStatusChanged StatusChanged;
        public StatusType Status { 
            get => status; 
            private set {
                if (value != status) {
                    StatusChanged?.Invoke(value);
                }
                status = value;
            } 
        }
        public int FailureThreshold { get; init; }
        public TimeSpan Timeout { get; init; }
        public TimeSpan Period { get; init; }
        public Uri Uri { get; init; }

        private int fails = 0;
        private StatusType status;

        public void Fail() {
            fails++;
            if (fails >= FailureThreshold + 1) {
                Status = StatusType.Dead;
                fails = FailureThreshold + 1;
            }
            else {
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
