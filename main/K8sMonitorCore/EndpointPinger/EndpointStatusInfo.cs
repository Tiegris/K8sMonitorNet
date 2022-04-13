using System;

namespace EndpointPinger;

public class EndpointStatusInfo
{
    public EndpointStatusInfo(IKey key, Endpoint endpoint) {
        Key = key;
        Uri = endpoint.Uri;
        StatusCode = endpoint.Status;
        LastChecked = endpoint.LastChecked;
        LastError = endpoint.LastError;
    }

    public IKey Key { get; internal init; }
    public Uri Uri { get; internal init; }
    public StatusType StatusCode { get; internal init; }
    public DateTime LastChecked { get; internal init; }
    public string? LastError { get; internal init; }
    public string StatusString => StatusCode.ToString();
}
