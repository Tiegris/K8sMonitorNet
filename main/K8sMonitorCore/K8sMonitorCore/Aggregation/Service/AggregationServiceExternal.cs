using EndpointPinger;
using K8sMonitorCore.Aggregation.Dto.Simple;
using KubernetesSyncronizer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{

    private static readonly Func<EndpointStatusInfo, bool> AllHealthy =
        b => b.StatusCode == StatusType.Healthy;

    private bool GetHealthOfNs(string ns) {
        var svcs = registry.Where(a => a.Key.Ns == ns);

        if (!svcs.Any())
            throw new KeyNotFoundException();

        foreach (var svc in svcs)
            if (!GetHealthOfSrv(svc.Key))
                return false;

        return true;
    }

    private bool GetHealthOfSrv(K8sKey selector) {
        if (!registry.TryGetValue(selector, out var svc))
            throw new KeyNotFoundException();

        var endpoints = stats.Where(a => selector.SrvEquals(a.Key));
        if (!endpoints.Any())
            return false;

        var hpa = svc.Hpa;
        if (hpa is { Enabled: true }) {
            return endpoints.Count(AllHealthy) * 100 / endpoints.Count() > hpa.Percentage;
        } else {
            return endpoints.Single().StatusCode == StatusType.Healthy;
        }
    }

    public bool GetHealthOf(string ns, string? srv = null) {
        if (srv is null) {
            return GetHealthOfNs(ns);
        } else {
            return GetHealthOfSrv(new K8sKey(ns, srv));
        }
    }

    public IEnumerable<SimpleStatusDto> GetHealthGroupBySrv() {
        foreach (var svc in registry) {
            yield return new SimpleStatusDto(GetHealthOfSrv(svc.Key), svc.Key.ToString());
        }
    }

    public IEnumerable<SimpleStatusDto> GetHealthGroupByNs() {
        var nss = registry.Select(a => a.Key.Ns).Distinct();
        foreach (var ns in nss) {
            yield return new SimpleStatusDto(GetHealthOfNs(ns), ns);
        }
    }

}
