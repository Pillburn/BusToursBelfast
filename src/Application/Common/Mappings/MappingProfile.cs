// Application/Common/Mappings/MappingProfile.cs
using AutoMapper;
using ToursApp.Domain.Entities;
using ToursApp.Application.Tours.DTOs;

namespace ToursApp.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //CreateMap<Tour, TourDto>()
          //  .ForMember(d => d.EndDate, opt => opt.MapFrom(s => s.StartDate.AddDays(s.DurationDays)));
    }
}