using Pinger;
using System;

namespace K8sMonitorCore.Domain
{
    public class ServiceHealthStatusDto
    {
        public ServiceHealthStatusDto(EndpointStatusInfo info) {
            LastChecked = info.LastChecked;
            StatusCode = info.StatusCode;
        }

        public DateTime LastChecked { get; internal init; }
        public StatusType StatusCode { get; internal init; }
        public string StatusString => StatusCode.ToString();
    }

    public static class ServiceHealthStatusDtoExtensions
    {
        public static ServiceHealthStatusDto? ToDto(this EndpointStatusInfo? info) {
            if (info is not null)
                return new ServiceHealthStatusDto(info);
            else
                return null;
        }
    }

}
