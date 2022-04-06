using K8sMonitorCore.Aggregation.Dto;
using KubernetesSyncronizer.Data;
using Pinger;
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

    private static bool GetHealthOfNs(string ns,
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
            statusInfos.Where(a => a.Name.GetSrvNs() == $"{srv}::{ns}");

        if (!endpoints.Any())
            throw new KeyNotFoundException();

        if (srv is null) {
            return GetHealthOfNs(ns, endpoints, resources);
        } else {
            return GetHealthOfSrv($"{srv}::{ns}", endpoints, resources);
        }
    }

    public IEnumerable<ShortStatusDto> GetHealthGroupBy(string groupBy) {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        return groupBy switch {
            "ns" or "namespace" =>
                    from i in statusInfos
                    group i by i.Name.GetNs() into nss
                    select new ShortStatusDto(
                        GetHealthOfNs(nss.Key, nss, resources),
                        nss.Key
                    ),
            "srv" or "service" =>
                    from i in statusInfos
                    group i by i.Name.GetSrvNs() into nss
                    select new ShortStatusDto(
                        GetHealthOfSrv(nss.Key, nss, resources),
                        nss.Key
                    ),
            _ => throw new ArgumentOutOfRangeException($"{groupBy} is not a valid groupBy type"),
        };
    }

}
