using System;
using BlazorBoilerplate.Bonus;
using Core.LibLog.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [AllowAnonymous]
    [Route("api/simple")]
    [ApiController]
    public class SimpleController : ControllerBase
    {
        private static readonly ILog Logger = LogProvider.For<SimpleController>();

        private readonly IBonusService _bonusService;

        public SimpleController(IBonusService bonusService)
        {
            _bonusService = bonusService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Message = "all OK",
                Value   = 42,
                Name    = _bonusService.AddStuff("my name"),
                Number  = _bonusService.CalculateBonus(10),
                Date    = _bonusService.GetDate()
            });
        }
    }
}