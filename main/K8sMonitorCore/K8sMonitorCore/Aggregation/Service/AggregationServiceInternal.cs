using K8sMonitorCore.Aggregation.Dto.Tree;
using KubernetesSyncronizer.Data;
using System.Collections.Generic;
using System.Linq;
using static KubernetesSyncronizer.Util.K8sKeyExtensions;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{
    public IList<NodeNsDto> TreeGrouping() {
        var grouping = from i in registry
                       group i by i.Key.GetSrvNs() into srvs
                       orderby srvs.Key
                       group srvs by srvs.Key.Ns;

        var y = grouping.Select(ns => new NodeNsDto(
            ns.Key,
            ns.Select(srv => new NodeSrvDto(
                srv.Key.Srv,                
                registry[srv.Key.GetSrvNs()],
                stats.Where(c => srv.Key.SrvEquals(c.Key)).Select(pod => new NodePodDto(pod)).ToList()
            )).ToList()
        )).OrderBy(o => o.Name).ToList();

        return y;
    }

}

