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
                _logger.LogInformation("Starting booking creation for tour: {TourId}", request.TourId);
                _logger.LogInformation("1. Validating tour exists...");
                // 1. Validate tour exists
                if (!Guid.TryParse(request.TourId.ToString(), out var tourGuid))
                {
                    _logger.LogWarning("❌ Invalid TourId format: {TourId}", request.TourId);
                    throw new ArgumentException($"Invalid TourId format: {request.TourId}");
                }
                var tour = await _tourRepository.GetTourByIdAsync(tourGuid);
                if (tour == null)
                {
                    _logger.LogWarning("Tour not found: {TourId}", request.TourId);
                    throw new NotFoundException($"Tour with ID {request.TourId} not found");
                }
                _logger.LogInformation("✅ Tour found: {TourName}", tour.Title);

                // 2. Calculate totals
                _logger.LogInformation("2. Calculating totals...");
                var totalParticipants = request.NumberOfParticipants.Adults + 
                                        request.NumberOfParticipants.Children + 
                                        request.NumberOfParticipants.Infants;
                
                var totalAmount = request.TourPrice * (request.NumberOfParticipants.Adults + 
                                                        request.NumberOfParticipants.Children);
                _logger.LogInformation("✅ Total: {TotalParticipants} participants, {TotalAmount} amount",totalParticipants,totalAmount); 
                // 3. Create booking (Status: Pending)
                _logger.LogInformation("3. Creating booking...");
                var booking = new Booking(request.CustomerName)
                {
                    TourId = Guid.Parse(request.TourId),
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
                _logger.LogInformation("✅ Booking created: {BookingId}", booking.Id);

                _logger.LogInformation("Booking created with ID: {BookingId}, Reference: {Reference}", 
                    booking.Id, booking.ReferenceNumber);

                // 4. Create Stripe Payment Intent
                _logger.LogInformation("4️⃣ Creating Stripe Payment Intent...");
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
                _logger.LogInformation("Payment intent result: Success={Success}, PaymentIntentId={PaymentIntentId}, ClientSecret={ClientSecret}",
                    paymentIntentResult.Success,
                    paymentIntentResult.PaymentIntentId ?? "NULL",
                    paymentIntentResult.ClientSecret ?? "NULL");

                if (!paymentIntentResult.Success)
                {
                    _logger.LogError("❌ Payment intent creation failed: {ErrorMessage}", paymentIntentResult.ErrorMessage);
                    throw new Exception(paymentIntentResult.ErrorMessage ?? "Payment creation failed");
                }

                // ✅ Check if ClientSecret is null
                if (string.IsNullOrEmpty(paymentIntentResult.ClientSecret))
                {
                    _logger.LogError("❌ ClientSecret is null or empty!");
                    throw new Exception("Payment intent client secret is missing");
                }

                // 5. Update booking with payment intent ID
                _logger.LogInformation("5️⃣ Updating booking with payment intent...");
                booking.PaymentIntentId = paymentIntentResult.PaymentIntentId ?? string.Empty;
                await _bookingRepository.UpdateBookingAsync(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("✅ Booking updated with payment intent");
                _logger.LogInformation("🔑 Returning ClientSecret: {ClientSecret}", paymentIntentResult.ClientSecret);
                // 6. Return result
                return new BookingPaymentResultDto
                {
                    Success = true,
                    Message = "Booking created. Please complete payment.",
                    BookingId = booking.Id,
                    ReferenceNumber = booking.ReferenceNumber,
                    ClientSecret = paymentIntentResult.ClientSecret ?? "null",
                    PaymentIntentId = paymentIntentResult.PaymentIntentId ?? "null",
                    TotalAmount = totalAmount,
                    Currency = "GBP",
                    RequiresPaymentMethod = true,
                    Status = 1
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