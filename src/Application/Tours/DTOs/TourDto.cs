// src/Application/Tours/DTOs/TourDto.cs
namespace ToursApp.Application.Tours.DTOs;

public record TourDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int DurationDays,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive);