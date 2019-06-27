using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BlazorBoilerplate.Bonus.Domain;
using BlazorBoilerplate.Bonus.ViewModels;

namespace BlazorBoilerplate.Bonus
{
    public class SampleDataMappingAction : IMappingAction<SampleData, SampleDataViewModel>
    {
        private readonly IBonusService _bonusService;

        public SampleDataMappingAction(IBonusService bonusService)
        {
            _bonusService = bonusService;
        }

        public void Process(SampleData source, SampleDataViewModel destination)
        {
            destination.Name = _bonusService.AddStuff(source.Name);
            destination.Number = _bonusService.CalculateBonus(source.Number);
        }
    }
}
