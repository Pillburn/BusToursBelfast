// Application/Bookings/Queries/GetBookingsByTour/GetBookingsByTourQuery.cs
using MediatR;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.Application.Bookings.Queries.GetBookingsByTour
{
    public class GetBookingsByTourQuery : IRequest<List<BookingListDto>>
    {
        public Guid TourId { get; set; }
    }
}