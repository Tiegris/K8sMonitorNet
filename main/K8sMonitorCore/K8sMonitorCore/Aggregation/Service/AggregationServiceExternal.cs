using EndpointPinger;
using K8sMonitorCore.Aggregation.Dto.Simple;
using KubernetesSyncronizer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static KubernetesSyncronizer.Util.KeyStringExtensions;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{

    private static readonly Func<EndpointStatusInfo, bool> AllHealthy =
        b => b.StatusCode == StatusType.Healthy;

    private static bool GetHealthOfSrv(string selector,
        IEnumerable<EndpointStatusInfo> endpoints,
        ICollection<MonitoredService> resources) {

        var hpa = resources.Single(a => a.Name == selector).Hpa;
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
            a.Name.GetSrvNs(),
            endpoints.Where(b => b.Name.GetSrvNs() == a.Name.GetSrvNs()),
            resources));
    }

    public bool GetHealthOf(string ns, string? srv = null) {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        var endpoints = srv is null ?
            statusInfos.Where(a => a.Name.GetNs() == ns) :
            statusInfos.Where(a => a.Name.GetSrvNs() == MakeKey(ns, srv));

        if (!endpoints.Any())
            throw new KeyNotFoundException();

        if (srv is null) {
            return GetHealthOfNs(endpoints, resources);
        } else {
            return GetHealthOfSrv(MakeKey(ns, srv), endpoints, resources);
        }
    }

    public IEnumerable<SimpleStatusDto> GetHealthGroupBySrv() {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        return from i in statusInfos
               group i by i.Name.GetSrvNs() into nss
               orderby nss.Key
               select new SimpleStatusDto(
                   GetHealthOfSrv(nss.Key, nss, resources),
                   nss.Key
               );
    }

    public IEnumerable<SimpleStatusDto> GetHealthGroupByNs() {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        return from i in statusInfos
               group i by i.Name.GetNs() into nss
               orderby nss.Key
               select new SimpleStatusDto(
                   GetHealthOfNs(nss, resources),
                   nss.Key
               );
    }

}
