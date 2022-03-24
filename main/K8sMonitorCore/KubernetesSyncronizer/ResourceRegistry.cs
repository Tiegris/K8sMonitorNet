using k8s.Models;
using KubernetesSyncronizer.Settings;
using KubernetesSyncronizer.Util;
using Microsoft.Extensions.Options;
using Pinger;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KubernetesSyncronizer
{
    public class ResourceRegistry
    {
        private readonly PingerManager pinger;
        private readonly Defaults defaults;

        public ResourceRegistry(PingerManager pinger, IOptions<Defaults> options) {
            this.pinger = pinger;
            this.defaults = options.Value;
        }

        private readonly ConcurrentDictionary<string, MonitoredService> map = new();

        public void Add(V1Service service) {
            var resource = service.ExtractMonitoredService(defaults);
            if (resource is null)
                return;

            map.TryAdd(resource.Name, resource);

            foreach (var (name, item) in resource.GetEndpoints())
                pinger.RegisterEndpoint(name, item);
        }


        public void Delete(V1Service service) {
            var serviceName = service.ExtractFullName();

            if (map.TryRemove(serviceName, out var monitoredService))
                foreach (var name in monitoredService.GetEndpointNames())
                    pinger.UnregisterEndpoint(name);
        }

        public ICollection<MonitoredService> Values => map.Values;

    }
}
