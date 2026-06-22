// Application/Bookings/Queries/GetBookingsByUser/GetBookingsByUserQuery.cs
using MediatR;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.Application.Bookings.Queries.GetBookingsByUser
{
    public class GetBookingsByUserQuery : IRequest<List<BookingListDto>>
    {
        public string Email { get; set; } = string.Empty;
    }
}