// Application/Bookings/Commands/DeleteBooking/DeleteBookingCommand.cs
using MediatR;

namespace ToursApp.Application.Bookings.Commands.DeleteBooking
{
    public class DeleteBookingCommand : IRequest<bool>
    {
        public Guid BookingId { get; set; }
    }
}