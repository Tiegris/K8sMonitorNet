using System;

namespace Pinger
{
    public class EndpointStatusInfo
    {
        public string Name { get; init; }
        public Uri Uri { get; init; }
        public StatusType StatusCode { get; init; }
        public string StatusString => StatusCode.ToString();
    }
}
