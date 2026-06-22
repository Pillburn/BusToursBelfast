using System.ComponentModel.DataAnnotations;
using ToursApp.Application.DTOs.Shared;

namespace ToursApp.Application.DTOs.BookingDTOs // ← Changed to avoid conflict
{
    public class CreateBookingRequest
    {
        [Required]
        public Guid TourId { get; set; }

        [Required]
        public string TourName { get; set; } = string.Empty;

        [Required]
        public decimal TourPrice { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public ParticipantCountDto NumberOfParticipants { get; set; } = new();

        [Required]
        public string PreferredDate { get; set; } = string.Empty;

        [Required]
        public string PickupLocation { get; set; } = string.Empty;

        public string SpecialRequests { get; set; } = string.Empty;
        public string? PassportNumber { get; set; }
        public string? DateOfBirth { get; set; }
        public string? EmergencyContact { get; set; }
        public string? TravelInsuranceDetails { get; set; }
    }
}