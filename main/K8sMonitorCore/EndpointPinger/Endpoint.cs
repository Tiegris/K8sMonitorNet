using System;
using System.Threading;

namespace Pinger;

public class Endpoint
{
    public DateTime LastChecked { get; private set; }
    public StatusType Status { get; private set; }
    public int FailureThreshold { get; init; }
    public TimeSpan Timeout { get; init; }
    public TimeSpan Period { get; init; }
    public Uri Uri { get; init; }

    private int fails = 0;

    /// <summary>
    /// Use for testing only.
    /// </summary>
    internal Endpoint() {
        Uri = new Uri("http://localhost:8080");
    }

    public Endpoint(int failureThreshold, TimeSpan timeout, TimeSpan period, Uri uri) {
        FailureThreshold = failureThreshold;
        Timeout = timeout;
        Period = period;
        Uri = uri;
    }

    internal void Fail() {
        LastChecked = DateTime.Now;
        var temp = Interlocked.Increment(ref fails);
        if (temp >= FailureThreshold + 1) {
            Status = StatusType.Dead;
            Interlocked.Exchange(ref fails, FailureThreshold + 1);
        } else {
            Status = StatusType.Dying;
        }
    }

    internal void Success() {
        LastChecked = DateTime.Now;
        Interlocked.Decrement(ref fails);
        if (fails <= 0) {
            Status = StatusType.Healthy;
            Interlocked.Exchange(ref fails, 0);
        } else {
            Status = StatusType.Recovering;
        }
    }

}
