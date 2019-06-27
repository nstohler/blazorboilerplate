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
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SampleData, SampleDataViewModel>()
                //.ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Moniker, opt => opt.MapFrom(s => $"{s.Name}-{s.Number}"))
                .AfterMap<SampleDataMappingAction>()
                ;
        }
    }
}
