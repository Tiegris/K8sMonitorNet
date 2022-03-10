using K8sMonitorCore.HostedServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace K8sMonitorCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        EndpointManager es;

        public DemoController(EndpointManager es)
        {
            this.es = es;
        }

        [HttpGet]
        public async Task CancelAsync()
        {
            
        }

    }
}
