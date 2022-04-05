using K8sMonitorCore.Aggregation.Service;
using K8sMonitorCore.Domain;
using K8sMonitorCore.Services;
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


    [HttpGet("List")]
    public IList<ServiceInfoDto> List() => aggregator.PlainList();

}
