using EndpointPinger;
using KubernetesSyncronizer.Data;
using KubernetesSyncronizer.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{
    private readonly List<EndpointStatusInfo> stats;
    private readonly ConcurrentDictionary<K8sKey, MonitoredService> registry;

    /// <summary>
    /// Use as transient service in the DI
    /// </summary>
    public AggregationService(ResourceRegistry resourceRegistry, PingerManager pingerManager) {
        stats = pingerManager.Scrape();
        registry = resourceRegistry.Map;
    }
}
