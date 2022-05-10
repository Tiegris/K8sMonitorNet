using EndpointPinger;
using KubernetesSyncronizer.Data;
using System;

namespace K8sMonitorCore.Aggregation.Dto.Tree;

public class NodePodDto
{
    public NodePodDto(EndpointStatusInfo info) {
        Name = (info.Key as K8sKey)?.Pod ?? "unknown";
        LastChecked = info.LastChecked;
        StatusCode = info.StatusCode;
        LastError = info.LastError;
    }
    public string Name { get; internal init; }
    public DateTime LastChecked { get; internal init; }
    public StatusType StatusCode { get; internal init; }
    public string? LastError { get; internal init; }
    public string StatusString => StatusCode.ToString();
}
