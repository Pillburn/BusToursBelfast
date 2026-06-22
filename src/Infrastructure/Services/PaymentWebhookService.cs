// Infrastructure/Services/PaymentWebhookService.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Application.Payments.Notifications;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Enums;
using ToursApp.Domain.Interfaces;

namespace ToursApp.Infrastructure.Services
{
    public class PaymentWebhookService : IPaymentWebhookService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IStripeEventMapper _stripeEventMapper;
        private readonly IMediator _mediator;
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentWebhookService> _logger;

        public PaymentWebhookService(
            IOptions<StripeSettings> stripeSettings,
            IStripeEventMapper stripeEventMapper,
            IMediator mediator,
            IBookingRepository bookingRepository,
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork,
            ILogger<PaymentWebhookService> logger)
        {
            _stripeSettings = stripeSettings.Value;
            _stripeEventMapper = stripeEventMapper;
            _mediator = mediator;
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> ProcessWebhookAsync(string jsonPayload, string signature)
        {
            try
            {
                // 1. Construct the Stripe event
                var stripeEvent = EventUtility.ConstructEvent(
                    jsonPayload,
                    signature,
                    _stripeSettings.WebhookSecret
                );

                _logger.LogInformation("Processing Stripe webhook: {EventType} {EventId}", 
                    stripeEvent.Type, stripeEvent.Id);

                // 2. Map to domain event type
                var domainEventType = _stripeEventMapper.MapToDomainEvent(stripeEvent.Type);

                // 3. Process based on the mapped event type
                switch (domainEventType)
                {
                    case PaymentEventType.PaymentSucceeded:
                        await HandlePaymentSucceededAsync(stripeEvent);
                        break;

                    case PaymentEventType.PaymentFailed:
                        await HandlePaymentFailedAsync(stripeEvent);
                        break;

                    case PaymentEventType.PaymentCanceled:
                        await HandlePaymentCanceledAsync(stripeEvent);
                        break;

                    case PaymentEventType.ChargeRefunded:
                        await HandleChargeRefundedAsync(stripeEvent);
                        break;

                    default:
                        _logger.LogWarning("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                        return true; // Return true for unhandled events (Stripe expects 2xx)
                }

                return true;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed: {Message}", ex.Message);
                return false;
            }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning(ex, "Unsupported Stripe event type");
                return true; // Return true to acknowledge receipt
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook: {Message}", ex.Message);
                return false;
            }
        }

       // Infrastructure/Services/PaymentWebhookService.cs
        private async Task HandlePaymentSucceededAsync(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
            if (paymentIntent == null)
            {
                _logger.LogWarning("PaymentIntent object is null in PaymentSucceeded event");
                return;
            }

            // Extract metadata
            var bookingId = paymentIntent.Metadata?.GetValueOrDefault("bookingId");
            if (string.IsNullOrEmpty(bookingId))
            {
                _logger.LogWarning("BookingId not found in payment intent metadata");
                return;
            }

            // Find the booking
            var booking = await _bookingRepository.GetBookingByIdAsync(Guid.Parse(bookingId));
            if (booking == null)
            {
                _logger.LogWarning("Booking not found: {BookingId}", bookingId);
                return;
            }

            // ✅ Create payment record
            var payment = new Payment(
                amount: (decimal)paymentIntent.Amount / 100,
                currency: paymentIntent.Currency,
                createdBy: "stripe_webhook")
            {
                PaymentIntentId = paymentIntent.Id,
                ChargeId = paymentIntent.LatestChargeId,
                BookingId = booking.Id,
                PaidAt = DateTime.UtcNow,
                Status = PaymentStatus.Succeeded
            };

            // ✅ Add cancellation token
            await _paymentRepository.AddPaymentAsync(payment, CancellationToken.None);

            // Update booking status
            booking.Status = BookingStatus.Confirmed;
            booking.ModifiedDate = DateTime.UtcNow;

            await _bookingRepository.UpdateBookingAsync(booking);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            // ✅ Create notification with ALL required parameters
            var notification = new PaymentSucceededNotification(
                paymentIntentId: paymentIntent.Id,
                chargeId: paymentIntent.LatestChargeId ?? string.Empty,
                bookingId: booking.Id.ToString(),
                referenceNumber: booking.ReferenceNumber,
                amount: (decimal)paymentIntent.Amount / 100,
                paidAt: DateTime.UtcNow,
                currency: paymentIntent.Currency
            );

            await _mediator.Publish(notification);

            _logger.LogInformation("Payment succeeded for booking: {BookingId}, PaymentIntent: {PaymentIntentId}", 
                booking.Id, paymentIntent.Id);
        }

        private async Task HandlePaymentFailedAsync(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
            if (paymentIntent == null)
            {
                _logger.LogWarning("PaymentIntent object is null in PaymentFailed event");
                return;
            }

            var bookingId = paymentIntent.Metadata?.GetValueOrDefault("bookingId");
            if (string.IsNullOrEmpty(bookingId))
            {
                _logger.LogWarning("BookingId not found in payment intent metadata");
                return;
            }

            var booking = await _bookingRepository.GetBookingByIdAsync(Guid.Parse(bookingId));
            if (booking == null)
            {
                _logger.LogWarning("Booking not found: {BookingId}", bookingId);
                return;
            }

            // Update booking status to Failed
            booking.Status = BookingStatus.Failed;
            booking.ModifiedDate = DateTime.UtcNow;

            await _bookingRepository.UpdateBookingAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            // Create payment record with failed status
            var payment = new Domain.Entities.Payment(
                amount: (decimal)paymentIntent.Amount / 100,
                currency: paymentIntent.Currency,
                createdBy: "stripe_webhook")
            {
                PaymentIntentId = paymentIntent.Id,
                BookingId = booking.Id,
                Status = PaymentStatus.Failed,
                ErrorMessage = paymentIntent.LastPaymentError?.Message ?? "Unknown error"
            };

            await _paymentRepository.AddPaymentAsync(payment, cancellationToken: CancellationToken.None);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogWarning("Payment failed for booking: {BookingId}, Error: {ErrorMessage}", 
                booking.Id, paymentIntent.LastPaymentError?.Message);
        }

        private async Task HandlePaymentCanceledAsync(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
            if (paymentIntent == null)
            {
                _logger.LogWarning("PaymentIntent object is null in PaymentCanceled event");
                return;
            }

            var bookingId = paymentIntent.Metadata?.GetValueOrDefault("bookingId");
            if (string.IsNullOrEmpty(bookingId))
            {
                _logger.LogWarning("BookingId not found in payment intent metadata");
                return;
            }

            var booking = await _bookingRepository.GetBookingByIdAsync(Guid.Parse(bookingId));
            if (booking == null)
            {
                _logger.LogWarning("Booking not found: {BookingId}", bookingId);
                return;
            }

            // Update booking status to Cancelled
            booking.Status = BookingStatus.Cancelled;
            booking.ModifiedDate = DateTime.UtcNow;

            await _bookingRepository.UpdateBookingAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Payment canceled for booking: {BookingId}", booking.Id);
        }

        private async Task HandleChargeRefundedAsync(Event stripeEvent)
        {
            var charge = stripeEvent.Data.Object as Stripe.Charge;
            if (charge == null)
            {
                _logger.LogWarning("Charge object is null in ChargeRefunded event");
                return;
            }

            var paymentIntentId = charge.PaymentIntentId;
            if (string.IsNullOrEmpty(paymentIntentId))
            {
                _logger.LogWarning("PaymentIntentId not found in charge");
                return;
            }

            // Find the payment
            var payment = await _paymentRepository.GetByPaymentIntentIdAsync(paymentIntentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for PaymentIntent: {PaymentIntentId}", paymentIntentId);
                return;
            }

            // Update payment status to Refunded
            payment.Status = PaymentStatus.Refunded;
            payment.LastRefundedAt = DateTime.UtcNow;
            payment.AmountRefunded = (decimal)charge.AmountRefunded / 100;

            await _paymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            // Update booking status if needed
            var booking = await _bookingRepository.GetBookingByIdAsync(payment.BookingId.GetValueOrDefault());
            if (booking != null)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.ModifiedDate = DateTime.UtcNow;
                await _bookingRepository.UpdateBookingAsync(booking);
                await _unitOfWork.SaveChangesAsync();
            }

            _logger.LogInformation("Charge refunded for booking: {BookingId}, Amount: {Amount}", 
                payment.BookingId, payment.AmountRefunded);
        }
    }
}