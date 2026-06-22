// Application/Bookings/Commands/CreateBooking/CreateBookingCommand.cs
using MediatR;
using ToursApp.Application.DTOs.Booking;
using ToursApp.Application.DTOs.Shared;

namespace ToursApp.Application.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommand : IRequest<BookingPaymentResultDto>
    {
        public Guid TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public decimal TourPrice { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public ParticipantCountDto NumberOfParticipants { get; set; } = new();
        public string PreferredDate { get; set; } = string.Empty;
        public string PickupLocation { get; set; } = string.Empty;
        public string SpecialRequests { get; set; } = string.Empty;
        public string? PassportNumber { get; set; }
        public string? DateOfBirth { get; set; }
        public string? EmergencyContact { get; set; }
        public string? TravelInsuranceDetails { get; set; }
        public string? PaymentMethod { get; set; }// "card", "google_pay", etc.
        public string PaymentIntentId { get; set; } = string.Empty;
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}