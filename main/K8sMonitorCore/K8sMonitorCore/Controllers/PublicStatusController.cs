using K8sMonitorCore.Aggregation.Dto;
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

    [HttpGet]
    public ActionResult IsOk([FromQuery] string ns, [FromQuery] string? srv) {
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

    [HttpGet("List")]
    public ActionResult<IEnumerable<ShortStatusDto>> ListAll([FromQuery] string groupBy) {
        try {
            return Ok(aggregator.GetHealthGroupBy(groupBy));
        } catch (Exception) {
            return StatusCode(500, "Unhandled Error");
        }
    }

}

