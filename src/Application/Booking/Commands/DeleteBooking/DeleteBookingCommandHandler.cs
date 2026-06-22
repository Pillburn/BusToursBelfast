// Application/Bookings/Commands/DeleteBooking/DeleteBookingCommandHandler.cs
using MediatR;
using ToursApp.Domain.Interfaces;

namespace ToursApp.Application.Bookings.Commands.DeleteBooking
{
    public class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookingCommandHandler(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
        {
            // 1. Get existing booking
            var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId);
            if (booking == null)
                return false;

            // 2. Check if booking can be deleted (e.g., only Pending status)
            if (booking.Status != Domain.Enums.BookingStatus.Pending)
                throw new InvalidOperationException("Cannot delete a booking that is already confirmed or completed");

            // 3. Delete the booking
            await _bookingRepository.DeleteBookingAsync(request.BookingId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}