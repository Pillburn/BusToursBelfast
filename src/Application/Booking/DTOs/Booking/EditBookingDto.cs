// Application/DTOs/Booking/EditBookingDto.cs
using ToursApp.Application.DTOs.Shared;

namespace ToursApp.Application.DTOs.Booking
{
    /// <summary>
    /// DTO for editing an existing booking
    /// All properties are optional to support partial updates
    /// </summary>
    public class EditBookingDto
    {
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
        public string? Status { get; set; } // For status updates
    }
}