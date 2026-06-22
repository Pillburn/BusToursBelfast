// Application/DTOs/Booking/BaseBookingDto.cs
using ToursApp.Application.DTOs.Shared;
using ToursApp.Domain.Enums;

namespace ToursApp.Application.DTOs.Booking
{
    /// <summary>
    /// Base DTO containing common booking properties
    /// Used as a foundation for all booking-related DTOs
    /// </summary>
    public abstract class BaseBookingDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;

        public Guid BookingId {get;set;}
        public string TourName { get; set; } = string.Empty;
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
        public int TotalParticipants { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}