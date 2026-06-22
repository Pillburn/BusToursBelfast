using MediatR;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.Application.Bookings.Queries.GetAllBookings
{
    public class GetAllBookingsQuery : IRequest<List<BookingListDto>>
    {
    }
}