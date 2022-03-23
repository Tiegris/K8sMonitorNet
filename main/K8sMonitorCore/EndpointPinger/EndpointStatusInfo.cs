using System;

namespace Pinger
{
    public class EndpointStatusInfo
    {
        public EndpointStatusInfo(string name, Uri uri, StatusType statusCode) {
            Name = name;
            Uri = uri;
            StatusCode = statusCode;
        }

        public string Name { get; init; }
        public Uri Uri { get; init; }
        public StatusType StatusCode { get; init; }
        public string StatusString => StatusCode.ToString();
    }
}
