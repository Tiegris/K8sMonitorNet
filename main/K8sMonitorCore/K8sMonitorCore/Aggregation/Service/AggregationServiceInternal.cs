using K8sMonitorCore.Aggregation.Dto;
using K8sMonitorCore.Aggregation.Dto.Tree;
using KubernetesSyncronizer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static KubernetesSyncronizer.Util.K8sKeyExtensions;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{
    public IList<NodeNsDto> TreeGrouping() {
        var grouping = from i in stats
                group i by (i.Key as K8sKey)?.GetSrvNs() into srvs
                group srvs by srvs.Key.Ns;

        var y = grouping.Select(ns => new NodeNsDto(
            ns.Key, 
            ns.Select(srv => new NodeSrvDto(
                srv.Key.Srv,
                registry.Single(c => c.Key.SrvEquals(srv.Key)).Value,
                srv.Select(pod => new NodePodDto(pod)).ToList()
            )).ToList()
        )).ToList();

        return y;
    }   

}

