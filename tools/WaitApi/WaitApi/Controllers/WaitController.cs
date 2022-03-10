using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WaitApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WaitController : ControllerBase
    {

        private readonly ILogger<WaitController> _logger;

        public WaitController(ILogger<WaitController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetFromQuery([FromQuery] int time = 0, [FromQuery] int status = 200)
        {
            Thread.Sleep(time);
            return StatusCode(status);
        }

        [HttpGet("{time}/{status}")]
        public ActionResult GetFromRoute([FromRoute] int time = 0, [FromRoute] int status = 200)
        {
            Thread.Sleep(time);
            return StatusCode(status);
        }

    }
}
