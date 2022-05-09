using EndpointPinger;
using k8s;
using k8s.Models;
using KubernetesSyncronizer.Data;
using KubernetesSyncronizer.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static KubernetesSyncronizer.Util.K8sKeyExtensions;

namespace KubernetesSyncronizer.Services;

public class ResourceRegistry
{
    private readonly PingerManager pinger;
    private readonly Extractor extractor;
    private readonly ILogger<ResourceRegistry> logger;

    public ResourceRegistry(PingerManager pinger, IOptions<Defaults> options, IKubernetes k8s, ILoggerFactory loggerFactory) {
        this.pinger = pinger;
        this.logger = loggerFactory.CreateLogger<ResourceRegistry>();
        this.extractor = new Extractor(options, k8s, this, loggerFactory);
    }

    public ConcurrentDictionary<K8sKey, MonitoredService> Map => map;
    private readonly ConcurrentDictionary<K8sKey, MonitoredService> map = new();
    private readonly ConcurrentDictionary<K8sKey, long> resourceVersions = new();

    internal bool ValidateEventOrder<T>(T item) where T : IMetadata<V1ObjectMeta> {
        K8sKey key = new(item.Namespace(), item.Name());
        long currentVersion = long.Parse(item.ResourceVersion());

        if (resourceVersions.TryGetValue(key, out long last)) {
            if (last < currentVersion) {
                resourceVersions.TryUpdate(key, currentVersion, last);
                return true;
            } else {
                logger.LogWarning("Out of order event detected.");
                return false;
            }
        } else {
            resourceVersions.TryAdd(key, currentVersion);
            return true;
        }
    }

    internal void AddPod(K8sKey serviceKey, V1Pod item) {
        if (item.Status.PodIP is null)
            return;

        if (item.Status.ContainerStatuses.Any(e => e.State.Terminated is not null))
            return;

        if (map.TryGetValue(serviceKey, out var service))
            if (service.TryGetEndpointForPod(item, out var key, out var ep)) {
                pinger.RegisterEndpoint(key, ep);
                logger.LogInformation("Pod {resource} added.", key);
            }
    }

    internal void DeletePod(K8sKey serviceKey, V1Pod item) {
        if (map.TryGetValue(serviceKey, out var service)) {
            var key = service.GetPodFullName(item);
            pinger.UnregisterEndpoint(key);
            logger.LogInformation("Pod {resource} delted.", key);
        }
    }

    internal void AddService(V1Service service) {
        if (extractor.TryExtractMonitoredService(service, out var resource) is false)
            return;

        if (map.TryAdd(resource.Key, resource))
            logger.LogInformation("Service {resource} added.", resource.Key);

        if (resource is { Errors.HasErrors: true })
            return;

        if (resource is { Hpa.Enabled: true }) {
            resource.PodMonitor?.StartWatching();
        } else {
            if (resource.TryGetEndpoint(out var key, out var ep))
                pinger.RegisterEndpoint(key, ep);
        }
    }

    internal void DeleteService(V1Service service) {
        var serviceName = service.ExtractKey();

        if (map.TryRemove(serviceName, out var monitoredService)) {
            monitoredService.Dispose();
            logger.LogInformation("Service {resource} deleted.", monitoredService.Key);
            foreach (var name in pinger.EndpointNames.Where(a => monitoredService.Key.SrvEquals(a)))
                pinger.UnregisterEndpoint(name);
        }
    }   

}
