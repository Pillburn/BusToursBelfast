using Microsoft.Extensions.Logging;
using Stripe;
using ToursApp.Application.Common.Interfaces;
// Infrastructure/Services/PaymentWebhookService.cs
using ToursApp.Infrastructure.Services;

namespace ToursApp.Infrastructure.Services;

public class PaymentWebhookService : IPaymentWebhookService
{
    private readonly ILogger<PaymentWebhookService> _logger;
    private readonly IStripeGate _stripeGateway;

    public PaymentWebhookService(
        IStripeGate stripeGateway,
        ILogger<PaymentWebhookService> logger)
    {
        _stripeGateway = stripeGateway;
        _logger = logger;
    }

    public async Task<bool> ProcessWebhookAsync(string json, string stripeSignature)
    {
        try
        {
            var stripeEvent = await _stripeGateway.ConstructEventAsync(json, stripeSignature);
            _logger.LogInformation("Received event: {Type}", stripeEvent.Type);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook failed");
            return false;
        }
    }

    private async Task<bool> HandlePaymentIntentSucceeded(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
        _logger.LogInformation("Payment succeeded: {Id}", paymentIntent.Id);
        return true;
    }

    private Task<bool> HandleUnsupportedEvent(Event stripeEvent)
    {
        _logger.LogWarning("Unhandled event: {EventType}", stripeEvent.Type);
        return Task.FromResult(true);
    }

    // Add placeholder methods for the other event handlers
    private Task<bool> HandlePaymentIntentFailed(Event stripeEvent)
        => Task.FromResult(true);
    
    private Task<bool> HandleChargeRefunded(Event stripeEvent)
        => Task.FromResult(true);
    
    private Task<bool> HandlePaymentIntentCanceled(Event stripeEvent)
        => Task.FromResult(true);
}