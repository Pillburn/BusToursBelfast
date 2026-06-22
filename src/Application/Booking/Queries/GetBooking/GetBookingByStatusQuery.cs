// Application/Bookings/Queries/GetBookingStatus/GetBookingStatusQuery.cs
using MediatR;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.Application.Bookings.Queries.GetBookingStatus
{
    public class GetBookingStatusQuery : IRequest<BookingStatusDto>
    {
        public string ReferenceNumber { get; set; } = string.Empty;
    }
}