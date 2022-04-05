using System;

namespace Pinger;

public class EndpointStatusInfo
{
    public EndpointStatusInfo(string name, Endpoint endpoint) {
        Name = name;
        Uri = endpoint.Uri;
        StatusCode = endpoint.Status;
        LastChecked = endpoint.LastChecked;
    }

    public string Name { get; internal init; }
    public Uri Uri { get; internal init; }
    public StatusType StatusCode { get; internal init; }
    public DateTime LastChecked { get; internal init; }
    public string StatusString => StatusCode.ToString();
}
