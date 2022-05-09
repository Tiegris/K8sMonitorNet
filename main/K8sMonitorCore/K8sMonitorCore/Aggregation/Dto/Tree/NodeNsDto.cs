using System.Collections.Generic;

namespace K8sMonitorCore.Aggregation.Dto.Tree;

public class NodeNsDto
{
    public NodeNsDto(string name, IList<NodeSrvDto> services) {
        Name = name;
        Services = services;
    }

    public string Name { get; init; }
    public IList<NodeSrvDto> Services { get; init; }
}
