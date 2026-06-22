// src/Domain/Entities/Booking.cs
using ToursApp.Domain.Common;
using ToursApp.Domain.Enums;


namespace ToursApp.Domain.Entities;

public class Booking : BaseEntity
{
    public Booking(string createdBy) : base(createdBy)
    {
        Id = Guid.NewGuid();
        ReferenceNumber = GenerateReferenceNumber();
        Status = BookingStatus.Pending;
        BookingDate =  DateTime.UtcNow;
    }
    
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;

    public Guid BookingId {get; set;}
    public BookingStatus Status { get; set; }
    public string ReferenceNumber {get;set;} = string.Empty; 
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber {get;set;} = string.Empty;
    public string ModifiedBy {get;set;} = string.Empty;
    public DateTime ModifiedDate {get;set;} = DateTime.UtcNow;
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public int NumberOfInfants { get; set; }
    public int TotalParticipants { get; set; }
    public DateOnly PreferredDate { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public int NumberOfPeople { get; private set; }
    public decimal TourPrice {get;set;}
    public decimal TotalPrice { get; set; }
    public bool IsConfirmed { get; private set; }
    public string Location{get;set;} = string.Empty;
    public string PickupLocation { get; set; } = string.Empty;
    public string SpecialRequests { get; set; } = string.Empty;
    public string? PassportNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? EmergencyContact { get; set; }
    public string? TravelInsuranceDetails { get; set; }
    
    // Navigation properties
    public Tour Tour { get; set; } = new Tour();
    public Payment? Payment { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
// ModifiedBy and ModifiedDate are already in Booking above
    // Primary constructor
    public Booking(
        Guid tourId,
        string customerName,
        string email,
        int numberOfPeople,
        decimal unitPrice,
        string createdBy) : base(createdBy)  // Pass to base
    {
        Id = Guid.NewGuid();
        TourId = tourId;
        CustomerName = customerName;
        Email = email;
        NumberOfPeople = numberOfPeople;
        TotalPrice = numberOfPeople * unitPrice;
        Validate();
    }

    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException("Booking is already confirmed");

        IsConfirmed = true;
    }

    public void UpdateDetails(string customerName, string customerEmail, int numberOfPeople)
    {
        CustomerName = customerName;
        Email = customerEmail;
        NumberOfPeople = numberOfPeople;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(CustomerName))
            throw new ArgumentException("Customer name is required");

        if (NumberOfPeople <= 0)
            throw new ArgumentException("Number of people must be positive");
    }
    

        public void Cancel()
        {
            if (Status == BookingStatus.Confirmed)
                throw new InvalidOperationException("Cannot cancel a completed booking");
            
            Status = BookingStatus.Cancelled;
        }

        public void MarkAsFailed()
        {
            Status = BookingStatus.Failed;
        }
    

     public Booking() : base("system") { }  // Temporary value for EF Core

//move to helpers whenever 
        private string GenerateReferenceNumber()
        {
            var now = DateTime.UtcNow;
            var random = new Random().Next(1000, 9999);
            return $"BKG-{now:yyyyMMdd}-{random:D4}";
        }
}
