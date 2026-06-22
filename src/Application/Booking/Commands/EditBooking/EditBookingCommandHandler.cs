// Application/Bookings/Commands/EditBooking/EditBookingCommandHandler.cs
using MediatR;
using ToursApp.Domain.Interfaces;
using ToursApp.Application.DTOs.BookingDTOs;
using ToursApp.Domain.Exceptions;
using ToursApp.Domain.Enums;
using ToursApp.Application.DTOs.Booking;

namespace ToursApp.Application.Bookings.Commands.EditBooking
{
    public class EditBookingCommandHandler : IRequestHandler<EditBookingCommand, BookingResponseDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditBookingCommandHandler(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BookingResponseDto> Handle(EditBookingCommand request, CancellationToken cancellationToken)
        {
            // 1. Get existing booking
            var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId);
            if (booking == null)
                throw new NotFoundException($"Booking with ID {request.BookingId} not found");

            // 2. Check if booking can be modified (e.g., only Pending status)
            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Cannot modify a booking that is already confirmed or completed");

            // 3. Update fields (only if provided)
            if (!string.IsNullOrEmpty(request.CustomerName))
                booking.CustomerName = request.CustomerName;

            if (!string.IsNullOrEmpty(request.Email))
                booking.Email = request.Email;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                booking.PhoneNumber = request.PhoneNumber;

            if (request.NumberOfParticipants != null)
            {
                booking.NumberOfAdults = request.NumberOfParticipants.Adults;
                booking.NumberOfChildren = request.NumberOfParticipants.Children;
                booking.NumberOfInfants = request.NumberOfParticipants.Infants;
                booking.TotalParticipants = booking.NumberOfAdults + 
                                           booking.NumberOfChildren + 
                                           booking.NumberOfInfants;
                booking.TotalPrice = booking.TourPrice * (booking.NumberOfAdults + booking.NumberOfChildren);
            }

            if (!string.IsNullOrEmpty(request.PreferredDate))
                booking.PreferredDate = DateOnly.Parse(request.PreferredDate);

            if (!string.IsNullOrEmpty(request.PickupLocation))
                booking.PickupLocation = request.PickupLocation;

            if (!string.IsNullOrEmpty(request.SpecialRequests))
                booking.SpecialRequests = request.SpecialRequests;

            if (request.PassportNumber != null)
                booking.PassportNumber = request.PassportNumber;

            if (request.DateOfBirth != null)
                booking.DateOfBirth = DateOnly.Parse(request.DateOfBirth);

            if (request.EmergencyContact != null)
                booking.EmergencyContact = request.EmergencyContact;

            if (request.TravelInsuranceDetails != null)
                booking.TravelInsuranceDetails = request.TravelInsuranceDetails;

            // 4. Update modified timestamp
            booking.ModifiedDate = DateTime.UtcNow;

            // 5. Save changes
            await _bookingRepository.UpdateBookingAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Map to response DTO
            return new BookingResponseDto
            {
                Id = booking.Id,
                ReferenceNumber = booking.ReferenceNumber,
                Status = booking.Status,
                Message = "Booking updated successfully",
                TotalPrice = booking.TotalPrice,
                TotalParticipants = booking.TotalParticipants,
                TourName = booking.TourName,
                PreferredDate = booking.PreferredDate.ToString("yyyy-MM-dd"),
                Email = booking.Email,
                BookingDate = booking.BookingDate
            };
        }
    }
}