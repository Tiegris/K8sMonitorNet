using K8sMonitorCore.Services;
using Microsoft.AspNetCore.Mvc;

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
