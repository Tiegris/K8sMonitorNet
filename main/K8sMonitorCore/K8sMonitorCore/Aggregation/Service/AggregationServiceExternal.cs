using EndpointPinger;
using K8sMonitorCore.Aggregation.Dto.Simple;
using KubernetesSyncronizer.Data;
using System;
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
        ICollection<MonitoredService> resources) {

        var hpa = resources.Single(a => a.Key.SrvEquals(selector)).Hpa;
        if (hpa is { Enabled: true }) {
            return endpoints.Count(AllHealthy) * 100 / endpoints.Count() > hpa.Percentage;
        } else {
            return endpoints.Single().StatusCode == StatusType.Healthy;
        }
    }

    private static bool GetHealthOfNs(
        IEnumerable<EndpointStatusInfo> endpoints,
        ICollection<MonitoredService> resources) {

        return endpoints.All(a => GetHealthOfSrv(
            a.Key,
            endpoints.Where(b => b.Key is K8sKey bk && bk.SrvEquals(a.Key)),
            resources));
    }

    public bool GetHealthOf(string ns, string? srv = null) {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        var endpoints = srv is null ?
            statusInfos.Where(a => a.Key is K8sKey ak && ak.Ns == ns) :
            statusInfos.Where(a => a.Key is K8sKey ak && ak.Ns == ns && ak.Srv == srv);

        if (!endpoints.Any())
            throw new KeyNotFoundException();

        if (srv is null) {
            return GetHealthOfNs(endpoints, resources);
        } else {
            return GetHealthOfSrv(new K8sKey(ns, srv), endpoints, resources);
        }
    }

    public IEnumerable<SimpleStatusDto> GetHealthGroupBySrv() {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        return from i in statusInfos
               group i by (i.Key as K8sKey)?.GetSrvNs() into nss
               orderby nss.Key
               select new SimpleStatusDto(
                   GetHealthOfSrv(nss.Key, nss, resources),
                   nss.Key.ToString()
               );
    }

    public IEnumerable<SimpleStatusDto> GetHealthGroupByNs() {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        return from i in statusInfos
               group i by (i.Key as K8sKey)?.Ns into nss
               orderby nss.Key
               select new SimpleStatusDto(
                   GetHealthOfNs(nss, resources),
                   nss.Key
               );
    }

}
