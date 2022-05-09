using K8sMonitorCore.Aggregation.Dto.Simple;
using K8sMonitorCore.Aggregation.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace K8sMonitorCore.Controllers;

[ApiController]
[Route("/api/public/status")]
public class PublicStatusController : ControllerBase
{
    private readonly AggregationService aggregator;

    public PublicStatusController(AggregationService aggregator) {
        this.aggregator = aggregator;
    }

    [HttpGet("{ns}")]
    public ActionResult Get([FromRoute] string ns) {
        try {
            if (aggregator.GetHealthOf(ns))
                return StatusCode(200, "All healthy within selection");
            else
                return StatusCode(533, "Dead services within selection");
        } catch (KeyNotFoundException) {
            return StatusCode(404, "Not Found");
        } catch (Exception) {
            return StatusCode(500, "Unhandled Error");
        }
    }

    [HttpGet("{ns}/{srv}")]
    public ActionResult Get([FromRoute] string ns, [FromRoute] string srv) {
        try {
            if (aggregator.GetHealthOf(ns, srv))
                return StatusCode(200, "All healthy within selection");
            else
                return StatusCode(533, "Dead services within selection");
        } catch (KeyNotFoundException) {
            return StatusCode(404, "Not Found");
        } catch (Exception) {
            return StatusCode(500, "Unhandled Error");
        }
    }

    [HttpGet("namespaces")]
    public ActionResult<IEnumerable<SimpleStatusDto>> GetByNs() {
        try {
            return Ok(aggregator.GetHealthGroupByNs());
        } catch (Exception) {
            return StatusCode(500, "Unhandled Error");
        }
    }

    [HttpGet("services")]
    public ActionResult<IEnumerable<SimpleStatusDto>> GetBySrv() {
        try {
            return Ok(aggregator.GetHealthGroupBySrv());
        } catch (Exception) {
            return StatusCode(500, "Unhandled Error");
        }
    }

}

