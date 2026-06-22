// Application/DTOs/Booking/BookingDetailsDto.cs
namespace ToursApp.Application.DTOs.Booking
{
    /// <summary>
    /// Full details DTO for a single booking
    /// Includes all available information
    /// </summary>
    public class BookingDetailsDto : BaseBookingDto
    {
        // Inherits all base properties
        // Can add additional details that aren't in the base
        public string TourId { get; set; } = string.Empty;
        public decimal TourPrice { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        
        // Computed properties for convenience
        public string AgeVerification 
        { 
            get 
            {
                if (string.IsNullOrEmpty(DateOfBirth))
                    return "Not provided";
                    
                var birthDate = DateOnly.Parse(DateOfBirth);
                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - birthDate.Year;
                if (birthDate > today.AddYears(-age)) age--;
                return $"{age} years old";
            }
        }
    }
}