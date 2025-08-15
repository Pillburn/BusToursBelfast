// Application/Services/PaymentWebhookService.cs
using MediatR;
using Stripe;
using ToursApp.Application.Common.Interfaces;
public class PaymentWebhookService : IPaymentWebhookService
{
    private readonly IStripeGate _stripeGate;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMediator _mediator;

    public PaymentWebhookService(
        IStripeGate stripeGateway,
        IPaymentRepository paymentRepository,
        IMediator mediator)
    {
        _stripeGate = stripeGateway;
        _paymentRepository = paymentRepository;
        _mediator = mediator;
    }


    public async Task ProcessWebhookEventAsync(string json, string stripeSignature)
    {
        var stripeEvent = await _stripeGate.ConstructEventAsync(json, stripeSignature);

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

    private async Task HandlePaymentIntentSucceeded(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        var charge = paymentIntent.Charges.First();

        await _mediator.Publish(new PaymentSucceededNotification(
            paymentIntent.Id,
            charge.Id,
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
