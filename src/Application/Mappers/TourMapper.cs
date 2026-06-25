// Application/Mappers/TourMapper.cs
using Riok.Mapperly.Abstractions;
using ToursApp.Domain.Entities;
using ToursApp.Application.Tours.DTOs;

namespace ToursApp.Application.Mappers
{
    [Mapper]
    public static partial class TourMapper
    {
        [MapProperty(nameof(Tour.CreatedAt), nameof(TourDto.CreatedAt))]
        [MapProperty(nameof(Tour.CreatedBy), nameof(TourDto.CreatedBy))]
        [MapProperty(nameof(Tour.LastModifiedAt), nameof(TourDto.LastModifiedAt))]
        [MapProperty(nameof(Tour.LastModifiedBy), nameof(TourDto.LastModifiedBy))]
        // Entity → DTO
        public static partial TourDto ToDto(Tour tour);
        
        // Collection mapping
        public static partial List<TourDto> ToDtos(List<Tour> tours);
    }
}