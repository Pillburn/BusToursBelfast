// Application/Bookings/Commands/CreateBooking/CreateBookingCommandHandler.cs
using MediatR;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Enums;
using ToursApp.Domain.Exceptions;
using ToursApp.Domain.Interfaces;
using ToursApp.Application.DTOs.Booking;
using ToursApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ToursApp.Application.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingPaymentResultDto>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<CreateBookingCommandHandler> _logger;

        public CreateBookingCommandHandler(
            IBookingRepository bookingRepository,
            ITourRepository tourRepository,
            IUnitOfWork unitOfWork,
            IPaymentGateway paymentGateway,
            ILogger<CreateBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _tourRepository = tourRepository;
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _logger = logger;
        }

        public async Task<BookingPaymentResultDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validate tour exists
                var tour = await _tourRepository.GetTourByIdAsync(request.TourId);
                if (tour == null)
                {
                    throw new NotFoundException($"Tour with ID {request.TourId} not found");
                }

                // 2. Calculate totals
                var totalParticipants = request.NumberOfParticipants.Adults + 
                                        request.NumberOfParticipants.Children + 
                                        request.NumberOfParticipants.Infants;
                
                var totalAmount = request.TourPrice * (request.NumberOfParticipants.Adults + 
                                                        request.NumberOfParticipants.Children);

                // 3. Create booking (Status: Pending)
                var booking = new Booking(request.CustomerName)
                {
                    TourId = request.TourId,
                    TourName = request.TourName,
                    TourPrice = request.TourPrice,
                    CustomerName = request.CustomerName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    NumberOfAdults = request.NumberOfParticipants.Adults,
                    NumberOfChildren = request.NumberOfParticipants.Children,
                    NumberOfInfants = request.NumberOfParticipants.Infants,
                    PreferredDate = DateOnly.Parse(request.PreferredDate),
                    PickupLocation = request.PickupLocation,
                    SpecialRequests = request.SpecialRequests,
                    PassportNumber = request.PassportNumber,
                    DateOfBirth = request.DateOfBirth != null ? DateOnly.Parse(request.DateOfBirth) : null,
                    EmergencyContact = request.EmergencyContact,
                    TravelInsuranceDetails = request.TravelInsuranceDetails,
                    TotalParticipants = totalParticipants,
                    PaymentIntentId =  request.PaymentIntentId,
                    TotalPrice = totalAmount,
                    Status = BookingStatus.Pending
                };

                await _bookingRepository.CreateBookingAsync(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Booking created with ID: {BookingId}, Reference: {Reference}", 
                    booking.Id, booking.ReferenceNumber);

                // 4. Create Stripe Payment Intent
                var paymentIntentResult = await _paymentGateway.CreatePaymentIntentAsync(
                    amount: totalAmount,
                    currency: "GBP",
                    metadata: new Dictionary<string, string>
                    {
                        { "bookingId", booking.Id.ToString() },
                        { "referenceNumber", booking.ReferenceNumber },
                        { "customerEmail", booking.Email },
                        { "customerName", booking.CustomerName }
                    },
                    cancellationToken: cancellationToken
                );

                // 5. Update booking with payment intent ID
                booking.PaymentIntentId = paymentIntentResult.PaymentIntentId;
                await _bookingRepository.UpdateBookingAsync(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 6. Return result
                return new BookingPaymentResultDto
                {
                    Success = true,
                    Message = "Booking created. Please complete payment.",
                    BookingId = booking.Id,
                    ReferenceNumber = booking.ReferenceNumber,
                    ClientSecret = paymentIntentResult.ClientSecret,
                    PaymentIntentId = paymentIntentResult.PaymentIntentId,
                    TotalAmount = totalAmount,
                    Currency = "GBP",
                    RequiresPaymentMethod = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking: {Message}", ex.Message);
                throw;
            }
        }
    }
}