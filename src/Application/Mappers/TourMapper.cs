// Application/Mappers/TourMapper.cs
using Riok.Mapperly.Abstractions;
using ToursApp.Domain.Entities;
using ToursApp.Application.Tours.DTOs;

namespace ToursApp.Application.Mappers
{
    [Mapper]
    public static partial class TourMapper
    {
        // Entity → DTO
        public static partial TourDto ToDto(Tour tour);
        
        // Collection mapping
        public static partial List<TourDto> ToDtos(List<Tour> tours);
    }
}