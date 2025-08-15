// src/Application/Bookings/DTOs/BookingDto.cs
namespace ToursApp.Application.Booking.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }

    // Customer Info
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }

    // Booking Details
    public DateTime BookingDate { get; set; }
    public int NumberOfPeople { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsConfirmed { get; set; }

    // Payment Information (for Stripe integration)
    public string PaymentStatus { get; set; }  // "pending", "paid", "failed"
    public string PaymentIntentId { get; set; }  // Stripe PaymentIntent ID
    public string ReceiptUrl { get; set; }  // Stripe receipt URL

    // Tour Summary (flattened from navigation property)
    public string TourName { get; set; }
    public DateTime TourStartDate { get; set; }
    public int TourDurationDays { get; set; }

};