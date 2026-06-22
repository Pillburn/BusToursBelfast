// Application/DTOs/Booking/BookingStatusDto.cs
namespace ToursApp.Application.DTOs.Booking
{
    /// <summary>
    /// Minimal DTO for checking booking status
    /// </summary>
    public class BookingStatusDto
    {
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TourName { get; set; } = string.Empty;
        public string PreferredDate { get; set; } = string.Empty;
        public int TotalParticipants { get; set; }
        public bool CanCancel { get; set; }
        public bool CanModify { get; set; }
    }
}