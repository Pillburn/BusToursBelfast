// DTO for creating a booking (from frontend)
using ToursApp.Domain.Entities;
using ToursApp.Domain.Enums;

public class CreateBookingRequest
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

    public static implicit operator Booking(CreateBookingRequest request)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            TourId = Guid.Parse(request.TourId),
            TourName = request.TourName,
            CustomerName = request.CustomerName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            NumberOfAdults = request.NumberOfParticipants.Adults,
            NumberOfChildren = request.NumberOfParticipants.Children,
            NumberOfInfants = request.NumberOfParticipants.Infants,
            PreferredDate = DateOnly.Parse(request.PreferredDate),
            PickupLocation = request.PickupLocation,
            SpecialRequests = request.SpecialRequests,
            PassportNumber = request.PassportNumber,
            DateOfBirth = request.DateOfBirth != null ? DateOnly.Parse(request.DateOfBirth) : null,
            EmergencyContact = request.EmergencyContact,
            TravelInsuranceDetails = request.TravelInsuranceDetails,
            BookingDate = DateTime.UtcNow,
            Status = BookingStatus.Pending,
            TotalParticipants = request.NumberOfParticipants.Adults + 
                               request.NumberOfParticipants.Children + 
                               request.NumberOfParticipants.Infants,
            TotalPrice = request.TourPrice * (request.NumberOfParticipants.Adults + request.NumberOfParticipants.Children)
        };
    }

}


public class ParticipantCountDto
{
    public int Adults { get; set; }
    public int Children { get; set; }
    public int Infants { get; set; }
}