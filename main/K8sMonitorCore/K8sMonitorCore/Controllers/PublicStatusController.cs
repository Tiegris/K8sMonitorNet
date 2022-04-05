using K8sMonitorCore.Aggregation.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace K8sMonitorCore.Controllers;

[ApiController]
[Route("[controller]")]
public class PublicStatusController : ControllerBase
{
    private readonly AggregationService aggregator;

    public PublicStatusController(AggregationService aggregator) {
        this.aggregator = aggregator;
    }

    [HttpGet()]
    public ActionResult IsOk([FromQuery] string srv, [FromQuery] string ns) {
        try {
            return StatusCode(200, "Ok");
        } catch (KeyNotFoundException e) {
            return StatusCode(404, "NotFound");
        } catch (Exception e) {
            return StatusCode(500, "");
        }
    }

}

