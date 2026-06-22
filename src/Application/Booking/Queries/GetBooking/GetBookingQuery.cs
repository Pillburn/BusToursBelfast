using MediatR;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.Application.Bookings.Queries.GetBooking
{
    public class GetBookingQuery : IRequest<BookingDetailsDto>
    {
        public Guid BookingId { get; set; }
    }
}