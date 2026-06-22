using MediatR;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.Application.Bookings.Queries.GetBookingByReference
{
    public class GetBookingByReferenceQuery : IRequest<BookingDetailsDto>
    {
        public string ReferenceNumber { get; set; } = string.Empty;
    }
}