// This is for internal application use - maps to domain
using ToursApp.Application.DTOs.Shared;

public class CreateBookingDto
{
    public string TourId { get; set; } = string.Empty;
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
}