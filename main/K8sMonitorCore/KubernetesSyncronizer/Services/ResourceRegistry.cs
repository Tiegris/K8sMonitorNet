using k8s;
using k8s.Models;
using KubernetesSyncronizer.Data;
using KubernetesSyncronizer.Util;
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

    public ResourceRegistry(PingerManager pinger, IOptions<Defaults> options, IKubernetes k8s) {
        this.pinger = pinger;
        this.extractor = new Extractor(options, k8s, this);
    }

    private readonly ConcurrentDictionary<string, MonitoredService> map = new();

    internal void AddPod(string serviceKey, V1Pod item) {
        if (map.TryGetValue(serviceKey, out var service))
            if (service.TryGetEndpointForPod(item, out var key, out var ep))
                pinger.RegisterEndpoint(key, ep);
    }

    internal void DeletePod(string serviceKey, V1Pod item) {
        if (map.TryGetValue(serviceKey, out var service)) {
            var key = service.GetPodFullName(item);
            pinger.UnregisterEndpoint(key);
        }
    }


    public void AddService(V1Service service) {
        if (extractor.TryExtractMonitoredService(service, out var resource) is false)
            return;

        map.TryAdd(resource.Name, resource);

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
            foreach (var name in pinger.EndpointNames.Where(a => a.Contains(monitoredService.Name)))
                pinger.UnregisterEndpoint(name);
        }
    }

    public ICollection<MonitoredService> Values => map.Values;

}
