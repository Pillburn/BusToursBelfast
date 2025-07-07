// src/Application/Bookings/DTOs/BookingDto.cs
namespace ToursApp.Application.Bookings.DTOs;

public record BookingDto(
    Guid Id,
    Guid TourId,
    string TourName,
    string CustomerName,
    string CustomerEmail,
    DateTime BookingDate,
    int NumberOfPeople,
    decimal TotalPrice,
    bool IsConfirmed);