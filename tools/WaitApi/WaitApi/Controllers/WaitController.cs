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
        public void Get()
        {
            return;
        }

        [HttpGet("{time}")]
        public void Get([FromRoute] int time)
        {
            Thread.Sleep(time);
            return;
        }
    }
}
