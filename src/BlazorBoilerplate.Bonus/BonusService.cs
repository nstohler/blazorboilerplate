using System;
using BlazorBoilerplate.Shared.Services;
using Core.LibLog.Logging;

namespace BlazorBoilerplate.Bonus
{
    internal class BonusService : IBonusService
    {
        private static readonly ILog Logger = LogProvider.For<BonusService>();

        private readonly ISharedDemoService _sharedDemoService;

        public BonusService(ISharedDemoService sharedDemoService)
        {
            _sharedDemoService = sharedDemoService;
        }

        public int CalculateBonus(int input)
        {
            Logger.InfoFormat("Calculating bonus for {input}", input);

            var rnd = new Random();
            return input + rnd.Next(1, 11);
        }

        public string AddStuff(string text)
        {
            Logger.InfoFormat("Adding stuff for {text}", text);

            var rnd = new Random();
            return text + new string('!', rnd.Next(1, 6));
        }

        public DateTime GetDate()
        {
            var date = _sharedDemoService.GetDate();
            return date;
        }
    }
}