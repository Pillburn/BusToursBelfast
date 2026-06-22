// Application/DTOs/Booking/BookingPaymentResultDto.cs
namespace ToursApp.Application.DTOs.Booking
{
    public class BookingPaymentResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid BookingId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty; // Stripe PaymentIntent client secret
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "GBP";
        public string RedirectUrl { get; set; } = string.Empty;
        public bool RequiresPaymentMethod { get; set; }
        public string? ErrorCode { get; set; }
    }
}