using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BlazorBoilerplate.Bonus;
using BlazorBoilerplate.Bonus.Domain;
using BlazorBoilerplate.Bonus.ViewModels;
using BlazorBoilerplate.Server.AutofacTest;
using Microsoft.AspNetCore.Mvc;

namespace BlazorBoilerplate.Server.Controllers
{
    [Route("api/demo")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly ITester       _tester;
        private readonly IBonusService _bonusService;
        private readonly IMapper       _mapper;

        public DemoController(ITester tester, IBonusService bonusService, IMapper mapper)
        {
            _tester       = tester;
            _bonusService = bonusService;
            _mapper       = mapper;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(new
            {
                Message = "wusel",
                Test    = _tester.RunTest("wusel"),
                Bonus   = _bonusService.CalculateBonus(100)
            });
        }

        [HttpGet("demo")]
        public IActionResult Demo()
        {
            var sampleData = new SampleData
            {
                Id     = 123,
                Name   = "Nicolas",
                Number = 100
            };

            var vm = _mapper.Map<SampleDataViewModel>(sampleData);

            return Ok(new
            {
                SampleData = sampleData,
                Vm         = vm,
                date       = _bonusService.GetDate()
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