using Pinger;
using System;
using System.Text.Json.Serialization;

namespace K8sMonitorCore.Aggregation.Dto;

public class ServiceHealthStatusDto
{
    public ServiceHealthStatusDto(EndpointStatusInfo info) {
        LastChecked = info.LastChecked;
        StatusCode = info.StatusCode;
        LastError = info.LastError;
    }

    public DateTime LastChecked { get; internal init; }
    [JsonIgnore]
    public StatusType StatusCode { get; internal init; }
    public string? LastError { get; internal init; }
    public string StatusString => StatusCode.ToString();
}

public static class ServiceHealthStatusDtoExtensions
{
    public static ServiceHealthStatusDto? ToDto(this EndpointStatusInfo? info) {
        if (info is null)
            return null;
        return new ServiceHealthStatusDto(info);
    }
}

