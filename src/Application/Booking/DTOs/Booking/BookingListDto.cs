// Application/DTOs/Booking/BookingListDto.cs
using ToursApp.Domain.Enums;
namespace ToursApp.Application.DTOs.Booking
{
    /// <summary>
    /// Lightweight DTO for listing bookings
    /// Inherits base properties and adds list-specific
    /// </summary>
    public class BookingListDto : BaseBookingDto
    {
        
        // Inherits all base properties
        // Add list-specific
        public string StatusDisplayName => Status.ToString();
        public string ShortReference => ReferenceNumber.Length > 10 
            ? ReferenceNumber.Substring(0, 10) + "..." 
            : ReferenceNumber;
        
        public bool IsPending => Status == BookingStatus.Pending;
        public bool IsConfirmed => Status == BookingStatus.Confirmed;
        public bool IsCancelled => Status == BookingStatus.Cancelled;
        public bool IsCompleted => Status == BookingStatus.Completed;
        
        // Quick participant summary
        public string ParticipantSummary 
        { 
            get 
            {
                var parts = new List<string>();
                if (NumberOfParticipants.Adults > 0) parts.Add($"{NumberOfParticipants.Adults} adults");
                if (NumberOfParticipants.Children > 0) parts.Add($"{NumberOfParticipants.Children} children");
                if (NumberOfParticipants.Infants > 0) parts.Add($"{NumberOfParticipants.Infants} infants");
                return string.Join(", ", parts);
            }
        }
    }
}