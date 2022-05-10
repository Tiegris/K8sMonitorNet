using K8sMonitorCore.Aggregation.Dto.Tree;
using K8sMonitorCore.Aggregation.Service;
using K8sMonitorCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace K8sMonitorCore.Controllers;

[ApiController]
public class EchoController : ControllerBase
{
    private readonly AutodiscoveryHostedService discovery;

    public EchoController(AutodiscoveryHostedService discovery) {
        this.discovery = discovery;
    }


    [HttpGet("echo")]
    [HttpGet("health")]
    public ActionResult Echo() => discovery.Watching ?
        StatusCode(200) :
        StatusCode(500);

}
