using K8sMonitorCore.Domain;
using K8sMonitorCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace K8sMonitorCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly AggregatorService aggregator;

        public StatusController(AggregatorService aggregator) {
            this.aggregator = aggregator;
        }

        [HttpGet("List")]
        public IList<ServiceInfoDto> List() => aggregator.PlainList();


    }
}
