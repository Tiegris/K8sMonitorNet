using EndpointPinger;
using KubernetesSyncronizer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K8sMonitorCore.Aggregation.Dto.Tree;

public class NodeSrvDto
{
    public NodeSrvDto(string name, MonitoredService ms, ICollection<NodePodDto> pods) {
        Name = name;
        Errors = ms.Errors;
        PingerSettings = new ServiceSettingsDto(
                ms.FailureThreshold,
                ms.Timeout,
                ms.Period,
                ms.Uri,
                ms.Hpa
            );
        Pods = pods;

        if (!Errors.HasErrors)
            if (PingerSettings is { Hpa.Enabled: true }) {
                if (pods.Any()) {
                    var p = Pods.OrderBy(b => b.LastChecked).First();
                    int percent = Pods.Count(b => b.StatusCode == StatusType.Healthy) * 100 / Pods.Count;
                    Health = new ServiceHealthStatusDto(
                        p.LastChecked,
                        percent > PingerSettings.Hpa.Percentage,
                        percent,
                        p.LastError
                    );
                } else {
                    Health = new ServiceHealthStatusDto(
                        DateTime.Now,
                        false,
                        null,
                        "No pods behind this service"
                        );
                }
            } else {
                var p = Pods.Single();
                Health = new ServiceHealthStatusDto(
                    p.LastChecked,
                    p.StatusCode == StatusType.Healthy,
                    null,
                    p.LastError
                );
            }
    }

    public string Name { get; init; }
    public ICollection<NodePodDto> Pods { get; init; }
    public ServiceHealthStatusDto? Health { get; init; }
    public ServiceConfigurationError Errors { get; init; }
    public ServiceSettingsDto? PingerSettings { get; init; }
}

