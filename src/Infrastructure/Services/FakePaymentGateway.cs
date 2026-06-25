// Infrastructure/Services/FakePaymentGateway.cs
using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Application.DTOs.Payment;

namespace ToursApp.Infrastructure.Services
{
    public class FakePaymentGateway : IPaymentGateway
    {
        
        private readonly ILogger<FakePaymentGateway> _logger;
        private readonly Random _random = new();
        private readonly StripeSettings _settings;
        private readonly Dictionary<string, PaymentIntentResult> _paymentIntents = new();
        private readonly object _lock = new();

        public FakePaymentGateway(
            IOptions<StripeSettings> settings,
            ILogger<FakePaymentGateway> logger)
        {
            _logger = logger;
            _settings = settings.Value;
        }
        public async Task<PaymentIntentResult> ConfirmPaymentIntentAsync(
            string paymentIntentId,
            string paymentMethodId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethodId
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.ConfirmAsync(
                    paymentIntentId, 
                    options, 
                    cancellationToken: cancellationToken
                );

                return new PaymentIntentResult
                {
                    Success = true,
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Status = paymentIntent.Status,
                    Amount = (decimal)(paymentIntent.Amount / 100m),
                    Currency = paymentIntent.Currency,
                    Metadata = paymentIntent.Metadata
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error confirming payment intent: {Message}", ex.Message);
                return new PaymentIntentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ErrorCode = ex.StripeError?.Code
                };
            }
        }

        public Task<PaymentIntentResult> CreatePaymentIntentAsync(
        decimal amount,
        string currency,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                // ✅ Generate a realistic-looking client secret
                var paymentIntentId = "pi_fake_" + Guid.NewGuid().ToString().Substring(0, 8);
                var secretPart = $"{paymentIntentId}_secret_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16)}";
                var clientSecret = $"{paymentIntentId}_secret_{secretPart}";
                var result = new PaymentIntentResult
                {
                    Success = true,
                    PaymentIntentId = paymentIntentId,
                    ClientSecret = clientSecret, // ✅ Must be a valid format
                    Status = "requires_payment_method",
                    Amount = amount,
                    Currency = currency,
                    Metadata = metadata
                };

                _paymentIntents[paymentIntentId] = result;
                return Task.FromResult(result);
            }
        }

        public Task<RefundResult> CreateRefundAsync(
            string paymentIntentId,
            decimal? amount = null,
            string? reason = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new RefundResult
            {
                PaymentIntentId = "pi_fake_" + Guid.NewGuid().ToString().Substring(0,8),
                Amount = amount ?? 0m,
                Status = "refunded",
                Success = false
            });
        }

        public async Task<PaymentIntentResult> GetPaymentIntentAsync(
            string paymentIntentId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

                return new PaymentIntentResult
                {
                    Success = true,
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Status = paymentIntent.Status,
                    Amount = (decimal)(paymentIntent.Amount / 100m),
                    Currency = paymentIntent.Currency,
                    Metadata = paymentIntent.Metadata
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error getting payment intent: {Message}", ex.Message);
                return new PaymentIntentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ErrorCode = ex.StripeError?.Code
                };
            }
        }
       public Task<WebhookResult> ProcessWebhookAsync(
            string jsonPayload,
            string signature,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    jsonPayload,
                    signature,
                    _settings.WebhookSecret
                );

                var result = new WebhookResult
                {
                    Success = true,
                    EventType = stripeEvent.Type
                };

                // Handle different event types
                switch (stripeEvent.Type)
                {
                case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntent != null)
                        {
                            result.PaymentIntentId = paymentIntent.Id;
                            result.Status = paymentIntent.Status;
                            
                            if (paymentIntent.Metadata != null)
                            {
                                paymentIntent.Metadata.TryGetValue("bookingId", out var bookingId);
                                paymentIntent.Metadata.TryGetValue("referenceNumber", out var referenceNumber);
                                result.BookingId = bookingId;
                                result.ReferenceNumber = referenceNumber;
                            }
                        }
                        break;

                    case "payment_intent.payment_failed":
                        var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (failedPaymentIntent != null)
                        {
                            result.PaymentIntentId = failedPaymentIntent.Id;
                            result.Status = failedPaymentIntent.Status;
                            result.ErrorMessage = failedPaymentIntent.LastPaymentError?.Message;
                        }
                        break;

                    case "charge.succeeded":
                        var charge = stripeEvent.Data.Object as Charge;
                        if (charge != null)
                        {
                            result.PaymentIntentId = charge.PaymentIntentId;
                            result.Status = "succeeded";
                        }
                        break;

                    case "payment_intent.canceled":
                        var canceledPaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (canceledPaymentIntent != null)
                        {
                            result.PaymentIntentId = canceledPaymentIntent.Id;
                            result.Status = canceledPaymentIntent.Status;
                        }
                        break;
                }

                return Task.FromResult(result);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook processing error: {Message}", ex.Message);
                return Task.FromResult(new WebhookResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
                
            }
        }
    }
}