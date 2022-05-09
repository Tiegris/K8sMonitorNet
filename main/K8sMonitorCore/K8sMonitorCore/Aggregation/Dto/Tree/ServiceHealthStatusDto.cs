using EndpointPinger;
using System;

namespace K8sMonitorCore.Aggregation.Dto.Tree;

public class ServiceHealthStatusDto
{
    public ServiceHealthStatusDto(DateTime lastChecked, bool healthy, int? healthyPercent, string? lastError) {
        LastChecked = lastChecked;
        Healthy = healthy;
        HealthyPercent = healthyPercent;
        LastError = lastError;
    }

    public DateTime LastChecked { get; internal init; }
    public bool Healthy { get; internal init; }
    public int? HealthyPercent { get; internal init; }
    public string? LastError { get; internal init; }
}
