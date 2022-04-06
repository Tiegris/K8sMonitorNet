using k8s;
using k8s.Models;
using KubernetesSyncronizer.Data;
using KubernetesSyncronizer.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pinger;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

    private readonly ConcurrentDictionary<string, MonitoredService> map = new();

    internal void AddPod(string serviceKey, V1Pod item) {
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

    internal void DeletePod(string serviceKey, V1Pod item) {
        if (map.TryGetValue(serviceKey, out var service)) {
            var key = service.GetPodFullName(item);
            pinger.UnregisterEndpoint(key);
            logger.LogInformation("Pod {resource} delted.", key);
        }
    }


    public void AddService(V1Service service) {
        if (extractor.TryExtractMonitoredService(service, out var resource) is false)
            return;

        if (map.TryAdd(resource.Name, resource))
            logger.LogInformation("Service {resource} added.", resource.Name);

        if (resource is { Errors.HasErrors: true })
            return;

        if (resource is { Hpa.Enabled: false }) {
            if (resource.TryGetEndpoint(out var key, out var ep))
                pinger.RegisterEndpoint(key, ep);
        } else {
            resource.PodMonitor?.StartWatching();
        }
    }


    public void DeleteService(V1Service service) {
        var serviceName = service.ExtractFullName();

        if (map.TryRemove(serviceName, out var monitoredService)) {
            monitoredService.Dispose();
            logger.LogInformation("Service {resource} deleted.", monitoredService.Name);
            foreach (var name in pinger.EndpointNames.Where(a => a.Contains(monitoredService.Name)))
                pinger.UnregisterEndpoint(name);
        }
    }

    public ICollection<MonitoredService> Values => map.Values;

}
