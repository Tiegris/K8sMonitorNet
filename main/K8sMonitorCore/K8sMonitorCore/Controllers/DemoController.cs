using Microsoft.AspNetCore.Mvc;
using Pinger;
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

        PingerManager pinger;

        public DemoController(PingerManager pinger) {
            this.pinger = pinger;
        }

        [HttpGet("Scrape")]
        public List<EndpointStatusInfo> Scrape()
        {
            return pinger.Scrape();
        }

        [HttpGet("Start")]
        public void New() {
            pinger.RegisterEndpoint("0_200", new Endpoint {
                FailureThreshold = 1,
                Period = new TimeSpan(0, 0, 10),
                Timeout = new TimeSpan(0, 0, 5),
                Uri = new Uri("http://localhost:9000/Wait/0/200"),
            });

            pinger.RegisterEndpoint("0_500", new Endpoint {
                FailureThreshold = 1,
                Period = new TimeSpan(0, 0, 20),
                Timeout = new TimeSpan(0, 0, 5),
                Uri = new Uri("http://localhost:9001/Wait/0/500"),
            });

            pinger.RegisterEndpoint("error", new Endpoint {
                FailureThreshold = 1,
                Period = new TimeSpan(0, 0, 10),
                Timeout = new TimeSpan(0, 0, 5),
                Uri = new Uri("http://localhost:8999/Wait/0/200"),
            });

            pinger.RegisterEndpoint("timeout", new Endpoint {
                FailureThreshold = 1,
                Period = new TimeSpan(0, 0, 6),
                Timeout = new TimeSpan(0, 0, 1),
                Uri = new Uri("http://localhost:9002/Wait/3000/200"),
            });

            pinger.RegisterEndpoint("wake_4", new Endpoint {
                FailureThreshold = 1,
                Period = new TimeSpan(0, 0, 10),
                Timeout = new TimeSpan(0, 0, 5),
                Uri = new Uri("http://localhost:9004/Wait/0/200"),
            });
        }


        [HttpGet("Cancel")]
        public void Cancel() {
            pinger.UnregisterEndpoint("timeout");
        }

    }
}
