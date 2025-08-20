// Application/Services/PaymentWebhookService.cs
using MediatR;
using Microsoft.Extensions.Logging;
using Stripe;
using ToursApp.Application.Common.Interfaces;
public class PaymentWebhookService : IPaymentWebhookService
{
    private readonly IStripeGate _stripeGate;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentWebhookService> _logger;

    public PaymentWebhookService(
        IStripeGate stripeGateway,
        IPaymentRepository paymentRepository,
        IMediator mediator,
        ILogger<PaymentWebhookService> logger)
    {
        _stripeGate = stripeGateway ?? throw new ArgumentNullException(nameof(stripeGateway));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task ProcessWebhookEventAsync(string json, string stripeSignature)
    {
        try
        {
            var stripeEvent = await _stripeGate.ConstructEventAsync(json, stripeSignature);

            if (stripeEvent?.Data?.Object == null)
            {
                _logger.LogWarning("Stripe event or data object is null");
                return;
            }

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":  // Now using raw strings
                    await HandlePaymentIntentSucceeded(stripeEvent);
                    break;

                case "charge.refunded":
                    await HandleChargeRefunded(stripeEvent);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentFailed(stripeEvent);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook event");
            throw;
         }
    }
    
    private async Task HandlePaymentIntentSucceeded(Event stripeEvent)
{
    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
    // Use the latest charge (modified for Stripe.net v40+)
    var chargeId = paymentIntent.LatestChargeId;
    
    await _mediator.Publish(new PaymentSucceededNotification(
            paymentIntent.Id,
            chargeId,  // Now using the charge ID directly
            paymentIntent.Amount / 100m,
            paymentIntent.Currency));
}

    private async Task HandleChargeRefunded(Event stripeEvent)
    {
        var refund = stripeEvent.Data.Object as Refund;

        // Get related payment from your repository
        var payment = await _paymentRepository.GetByChargeIdAsync(refund.ChargeId);

        if (payment == null)
        {
            _logger.LogWarning($"No payment found for charge: {refund.ChargeId}");
            return;
        }

        // Publish domain notification
        await _mediator.Publish(new PaymentRefundedNotification(
            payment.Id,
            refund.Id,
            refund.Amount / 100m,  // Convert from cents
            refund.ChargeId,
            refund.Reason ?? "unknown"));

        // Update domain entity
        payment.RecordRefund(
            amount: refund.Amount / 100m,
            refundedAt: DateTime.UtcNow,
            stripeRefundId: refund.Id);

        await _paymentRepository.UpdateAsync(payment);
    }
    
    private async Task HandlePaymentFailed(Event stripeEvent)
{
    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
    
    // Get payment from repository
    var payment = await _paymentRepository.GetByPaymentIntentIdAsync(paymentIntent.Id);
    
    if (payment == null)
    {
        _logger.LogWarning($"No payment found for PaymentIntent: {paymentIntent.Id}");
        return;
    }

    string failureReason = paymentIntent.LastPaymentError?.Message ?? "Unknown failure reason";
    
    // Publish domain notification
    await _mediator.Publish(new PaymentFailedNotification(
        payment.Id,
        paymentIntent.Id,
        failureReason,
        paymentIntent.LastPaymentError?.Code));
    
    // Update domain entity
    payment.MarkAsFailed(failureReason);
    await _paymentRepository.UpdateAsync(payment);
}
}
