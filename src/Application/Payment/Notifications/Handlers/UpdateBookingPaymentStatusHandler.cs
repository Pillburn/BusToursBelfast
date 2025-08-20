using MediatR;
using Microsoft.Extensions.Logging;
using ToursApp.Domain.Interfaces;
using ToursApp.Domain.Entities;
using ToursApp.Application.Common.Interfaces;
public class UpdateBookingPaymentStatusHandler
    : INotificationHandler<PaymentSucceededNotification>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<UpdateBookingPaymentStatusHandler> _logger;
    private readonly IUnitOfWork _unitOfWork; // Optional but recommended

    public UpdateBookingPaymentStatusHandler(
        IBookingRepository bookingRepository,
        IPaymentRepository paymentRepository,
        ILogger<UpdateBookingPaymentStatusHandler> logger,
        IUnitOfWork unitOfWork = null)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        PaymentSucceededNotification notification,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get booking
            var booking = await _bookingRepository
                .GetByPaymentIntentIdAsync(notification.PaymentIntentId, cancellationToken);

            if (booking == null)
            {
                _logger.LogWarning("No booking found for PaymentIntent: {PaymentIntentId}",
                    notification.PaymentIntentId);
                return;
            }

            // 2. Create payment record
            var payment = new Payment(
                amount: notification.Amount,
                currency: notification.Currency,
                createdBy: "payment_webhook")
            {
                PaymentIntentId = notification.PaymentIntentId,
                ChargeId = notification.ChargeId,
                BookingId = booking?.Id,
                PaidAt = DateTime.UtcNow
            };

            payment.MarkAsPaid(notification.ChargeId);

            await _paymentRepository.AddPaymentAsync(payment, cancellationToken);

            // 3. Update booking status
            // BOOKING STATUS UPDATE - add this simple method to Booking.cs
            booking.Status = BookingStatus.Confirmed; // Direct status update


            // 4. Save changes
            if (_unitOfWork != null)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                await _bookingRepository.SaveChangesAsync(cancellationToken);
                await _paymentRepository.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Successfully processed payment for booking {BookingId}", booking.Id);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment notification");
            throw;
        }
    }
}