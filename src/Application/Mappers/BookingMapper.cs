// ToursApp.Application/Mappers/BookingMapper.cs
using Riok.Mapperly.Abstractions;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Enums;
using ToursApp.Application.DTOs.BookingDTOs;
using ToursApp.Application.DTOs.Shared;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.Application.Mappers
{
    [Mapper]
    public static partial class BookingMapper
    {
        public static Booking ToEntity(CreateBookingRequest request)
        {
            if (request == null) return null!;

            return new Booking(
                tourId: request.TourId,
                customerName: request.CustomerName,
                email: request.Email,
                numberOfPeople: request.NumberOfParticipants.Adults + 
                                request.NumberOfParticipants.Children + 
                                request.NumberOfParticipants.Infants,
                unitPrice: request.TourPrice,
                createdBy: "system"
            )
            {
                TourName = request.TourName,
                TourPrice = request.TourPrice,
                PhoneNumber = request.PhoneNumber,
                NumberOfAdults = request.NumberOfParticipants.Adults,
                NumberOfChildren = request.NumberOfParticipants.Children,
                NumberOfInfants = request.NumberOfParticipants.Infants,
                TotalParticipants = request.NumberOfParticipants.Adults + 
                                   request.NumberOfParticipants.Children + 
                                   request.NumberOfParticipants.Infants,
                PreferredDate = DateOnly.Parse(request.PreferredDate),
                PickupLocation = request.PickupLocation,
                SpecialRequests = request.SpecialRequests,
                PassportNumber = request.PassportNumber,
                DateOfBirth = request.DateOfBirth != null ? DateOnly.Parse(request.DateOfBirth) : null,
                EmergencyContact = request.EmergencyContact,
                TravelInsuranceDetails = request.TravelInsuranceDetails,
                ReferenceNumber = GenerateReference()
            };
        }
        public static BookingResponseDto ToDtos(Booking booking)
        {
            if (booking == null ) return null!;

            return new BookingResponseDto
            {
                
            };
        }
        public static BookingDetailsDto ToDetailsDto(Booking booking)
        {
            if (booking == null ) return null!;

            return new BookingDetailsDto
            {
                
            };
        }
        public static BookingListDto ToListDto(Booking booking)
        {
            if (booking == null ) return null!;

            return new BookingListDto
            {
                
                Id = booking.Id,
                ReferenceNumber = booking.ReferenceNumber,
                TourName = booking.TourName,
                Email = booking.Email,
                PhoneNumber = booking.PhoneNumber,
                NumberOfParticipants = new ParticipantCountDto
                {
                    Adults = booking.NumberOfAdults,
                    Children = booking.NumberOfChildren,
                    Infants = booking.NumberOfInfants
                },
                PreferredDate = booking.PreferredDate.ToString("yyyy-MM-dd"),
                PickupLocation = booking.PickupLocation,
                SpecialRequests = booking.SpecialRequests,
                PassportNumber = booking.PassportNumber,
                DateOfBirth = booking.DateOfBirth?.ToString("yyyy-MM-dd"),
                EmergencyContact = booking.EmergencyContact,
                TravelInsuranceDetails = booking.TravelInsuranceDetails,
                TotalParticipants = booking.TotalParticipants,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                BookingDate = booking.BookingDate,
                ModifiedDate = booking.ModifiedDate
            };
        }
        private static string GenerateReference()
        {
            var now = DateTime.UtcNow;
            var random = new Random().Next(1000, 9999);
            return $"BKG-{now:yyyyMMdd}-{random:D4}";
        }
    }
}