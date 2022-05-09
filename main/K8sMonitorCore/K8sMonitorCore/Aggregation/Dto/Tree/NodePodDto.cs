using EndpointPinger;
using System;

namespace K8sMonitorCore.Aggregation.Dto.Tree;

public class NodePodDto
{
    public NodePodDto(EndpointStatusInfo info) {
        LastChecked = info.LastChecked;
        StatusCode = info.StatusCode;
        LastError = info.LastError;
    }

    public DateTime LastChecked { get; internal init; }
    public StatusType StatusCode { get; internal init; }
    public string? LastError { get; internal init; }
    public string StatusString => StatusCode.ToString();
}
