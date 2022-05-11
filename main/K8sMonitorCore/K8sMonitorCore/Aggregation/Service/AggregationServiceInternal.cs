using K8sMonitorCore.Aggregation.Dto.Tree;
using System.Collections.Generic;
using System.Linq;
using static KubernetesSyncronizer.Util.K8sKeyExtensions;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{
    public IList<NodeNsDto> TreeGrouping() {
        var grouping = from i in registry
                       group i by i.Key.GetSvcNs() into svcs
                       orderby svcs.Key
                       group svcs by svcs.Key.Ns;

        var y = grouping.Select(ns => new NodeNsDto(
            ns.Key,
            ns.Select(svc => new NodeSvcDto(
                svc.Key.Svc,
                registry[svc.Key.GetSvcNs()],
                stats.Where(c => svc.Key.SvcEquals(c.Key)).Select(pod => new NodePodDto(pod)).ToList()
            )).ToList()
        )).OrderBy(o => o.Name).ToList();

        return y;
    }

}

