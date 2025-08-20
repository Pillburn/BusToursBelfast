// Infrastructure/Services/StripeGateway.cs
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using ToursApp.Application.Common.Interfaces;

namespace ToursApp.Infrastructure.Services;

public class StripeGateway : IStripeGate
{
    private readonly StripeSettings _stripeSettings;
    private readonly ILogger<StripeGateway> _logger;

    public StripeGateway(IOptions<StripeSettings> stripeSettings, ILogger<StripeGateway> logger)
    {
        _stripeSettings = stripeSettings.Value;
        _logger = logger;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<Event> ConstructEventAsync(string json, string stripeSignature)
    {
        try
        {
            return EventUtility.ConstructEvent(
                json,
                stripeSignature,
                _stripeSettings.WebhookSecret
            );
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe event construction failed");
            throw;
        }
    }

    public async Task<Stripe.PaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
    {
        var service = new PaymentIntentService();
        return await service.GetAsync(paymentIntentId);
    }

    public async Task<Refund> CreateRefundAsync(string chargeId, decimal amount, string reason)
    {
        var options = new RefundCreateOptions
        {
            Charge = chargeId,
            Amount = (long)(amount * 100), // Convert to cents
            Reason = reason switch
            {
                "requested_by_customer" => RefundReasons.RequestedByCustomer,
                "duplicate" => RefundReasons.Duplicate,
                _ => RefundReasons.RequestedByCustomer
            }
        };

        var service = new RefundService();
        return await service.CreateAsync(options);
    }

    public async Task<ToursApp.Domain.Entities.PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency,string createdBy)
{
    var options = new Stripe.PaymentIntentCreateOptions
    {
        Amount = (long)(amount * 100), // Convert to cents
        Currency = currency.ToLower(),
        PaymentMethodTypes = new List<string> { "card" }
    };

    var service = new Stripe.PaymentIntentService();
    var stripeIntent = await service.CreateAsync(options);

        // Convert Stripe model to domain model
    return new ToursApp.Domain.Entities.PaymentIntent(
            stripeIntent.Id,
            amount,
            currency,
            stripeIntent.ClientSecret,
            createdBy,
            MapStatus(stripeIntent.Status)
            );
}

private PaymentIntentStatus MapStatus(string stripeStatus) => stripeStatus switch
{
    "requires_payment_method" => PaymentIntentStatus.RequiresPaymentMethod,
    "requires_confirmation" => PaymentIntentStatus.RequiresConfirmation,
    "requires_action" => PaymentIntentStatus.RequiresAction,
    "processing" => PaymentIntentStatus.Processing,
    "succeeded" => PaymentIntentStatus.Succeeded,
    "canceled" => PaymentIntentStatus.Canceled,
    _ => PaymentIntentStatus.RequiresPaymentMethod
};

}