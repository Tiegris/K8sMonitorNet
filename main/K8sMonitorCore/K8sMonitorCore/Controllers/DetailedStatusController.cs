using K8sMonitorCore.Aggregation.Dto.Tree;
using K8sMonitorCore.Aggregation.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace K8sMonitorCore.Controllers;

[ApiController]
[Route("[controller]")]
public class DetailedStatusController : ControllerBase
{
    private readonly AggregationService aggregator;

    public DetailedStatusController(AggregationService aggregator) {
        this.aggregator = aggregator;
    }


    [HttpGet("Tree")]
    public IList<NodeNsDto> List() => aggregator.TreeGrouping();


}
