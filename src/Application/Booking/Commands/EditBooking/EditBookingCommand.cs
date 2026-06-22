// Application/Bookings/Commands/EditBooking/EditBookingCommand.cs
using MediatR;
using ToursApp.Application.DTOs.Booking;
using ToursApp.Application.DTOs.Shared;
using ToursApp.Domain.Enums;

namespace ToursApp.Application.Bookings.Commands.EditBooking
{
    public class EditBookingCommand : IRequest<BookingResponseDto>
    {
        public Guid BookingId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public ParticipantCountDto? NumberOfParticipants { get; set; }
        public string? PreferredDate { get; set; }
        public string? PickupLocation { get; set; }
        public string? SpecialRequests { get; set; }
        public string? PassportNumber { get; set; }
        public string? DateOfBirth { get; set; }
        public string? EmergencyContact { get; set; }
        public string? TravelInsuranceDetails { get; set; }
        public BookingStatus? NewStatus{get;set;}
    }
}