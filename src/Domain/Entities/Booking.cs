// src/Domain/Entities/Booking.cs
using ToursApp.Domain.Common;

namespace ToursApp.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid PaymentId { get; set; }
    public Payment? Payment { get; set; }
    public Guid TourId { get; private set; }
    public BookingStatus Status { get; set; }
    public string CustomerName { get; private set; }
    public string CustomerEmail { get; private set; }
    public DateTime BookingDate { get; private set; } = DateTime.UtcNow;
    public int NumberOfPeople { get; private set; }
    public decimal TotalPrice { get; private set; }
    public bool IsConfirmed { get; private set; }
    // Navigation property
    public Tour Tour { get; private set; }
    private Booking(string createdBy) : base(createdBy) { }

    // Primary constructor
    public Booking(
        Guid tourId,
        string customerName,
        string customerEmail,
        int numberOfPeople,
        decimal unitPrice,
        string createdBy) : base(createdBy)  // Pass to base
    {
        Id = Guid.NewGuid();
        TourId = tourId;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
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
        CustomerEmail = customerEmail;
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
    
     private Booking() : base("system") { }  // Temporary value for EF Core
}
