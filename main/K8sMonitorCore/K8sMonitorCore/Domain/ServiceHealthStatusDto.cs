using Pinger;
using System;

namespace K8sMonitorCore.Domain
{
    public class ServiceHealthStatusDto
    {
        public ServiceHealthStatusDto(DateTime lastChecked, StatusType statusCode) {
            LastChecked = lastChecked;
            StatusCode = statusCode;
        }

        public DateTime LastChecked { get; internal init; }
        public StatusType StatusCode { get; internal init; }
        public string StatusString => StatusCode.ToString();
    }


}
