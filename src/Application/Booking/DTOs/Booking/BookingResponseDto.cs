// Application/DTOs/Booking/BookingResponseDto.cs
namespace ToursApp.Application.DTOs.Booking
{
    /// <summary>
    /// Response DTO for booking operations (Create, Update)
    /// Includes confirmation information
    /// </summary>
    public class BookingResponseDto : BaseBookingDto
    {
        public DateTime? ProcessedAt { get; set; }
        public string? ProcessingTimeMs { get; set; }
        
        //not set by the entity/null handling
        public string Message { get; set; } = string.Empty;
        public string ConfirmationLink { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
        public string OperationType { get; set; } = "Created";
    }
}