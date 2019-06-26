using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorBoilerplate.Server.AutofacTest;
using Microsoft.AspNetCore.Mvc;

namespace BlazorBoilerplate.Server.Controllers
{
    [Route("api/demo")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly ITester _tester;

        public DemoController(ITester tester)
        {
            _tester = tester;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(new
            {
                Message = "wusel",
                Test    = _tester.RunTest("wusel"),
            });
        }

        [HttpPost("dusel")]
        public IActionResult Dusel()
        {
            return Ok(new
            {
                Message = "dusel"
            });
        }

        [HttpPost("hallo")]
        public IActionResult Hallo()
        {
            return Ok(new
            {
                Message = "dusel"
            });
        }
    }
}