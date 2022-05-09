using EndpointPinger;
using K8sMonitorCore.Aggregation.Dto.Simple;
using KubernetesSyncronizer.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static KubernetesSyncronizer.Util.K8sKeyExtensions;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{

    private static readonly Func<EndpointStatusInfo, bool> AllHealthy =
        b => b.StatusCode == StatusType.Healthy;

    private static bool GetHealthOfSrv(IKey selector,
        IEnumerable<EndpointStatusInfo> endpoints,
        ConcurrentDictionary<K8sKey, MonitoredService> resources) {

        var hpa = resources.Single(a => a.Key.SrvEquals(selector)).Value.Hpa;
        if (hpa is { Enabled: true }) {
            return endpoints.Count(AllHealthy) * 100 / endpoints.Count() > hpa.Percentage;
        } else {
            return endpoints.Single().StatusCode == StatusType.Healthy;
        }
    }

    private static bool GetHealthOfNs(
        IEnumerable<EndpointStatusInfo> endpoints,
        ConcurrentDictionary<K8sKey, MonitoredService> resources) {

        return endpoints.All(a => GetHealthOfSrv(
            a.Key,
            endpoints.Where(b => b.Key is K8sKey bk && bk.SrvEquals(a.Key)),
            resources));
    }

    public bool GetHealthOf(string ns, string? srv = null) {
        var endpoints = srv is null ?
            stats.Where(a => a.Key is K8sKey ak && ak.Ns == ns) :
            stats.Where(a => a.Key is K8sKey ak && ak.Ns == ns && ak.Srv == srv);

        if (!endpoints.Any())
            throw new KeyNotFoundException();

        if (srv is null) {
            return GetHealthOfNs(endpoints, registry);
        } else {
            return GetHealthOfSrv(new K8sKey(ns, srv), endpoints, registry);
        }
    }

    public IEnumerable<SimpleStatusDto> GetHealthGroupBySrv() {
        return from i in stats
               group i by (i.Key as K8sKey)?.GetSrvNs() into nss
               orderby nss.Key
               select new SimpleStatusDto(
                   GetHealthOfSrv(nss.Key, nss, registry),
                   nss.Key.ToString()
               );
    }

    public IEnumerable<SimpleStatusDto> GetHealthGroupByNs() {
        return from i in stats
               group i by (i.Key as K8sKey)?.Ns into nss
               orderby nss.Key
               select new SimpleStatusDto(
                   GetHealthOfNs(nss, registry),
                   nss.Key
               );
    }

}
