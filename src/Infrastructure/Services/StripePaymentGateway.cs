// Infrastructure/Services/StripePaymentGateway.cs
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Application.DTOs.Payment;
using ToursApp.Infrastructure.Services;


namespace ToursApp.Infrastructure.Services
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly StripeSettings _settings;
        private readonly ILogger<StripePaymentGateway> _logger;

        public StripePaymentGateway(
            IOptions<StripeSettings> settings,
            ILogger<StripePaymentGateway> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            StripeConfiguration.ApiKey = _settings.SecretKey;
        }

        public async Task<PaymentIntentResult> CreatePaymentIntentAsync(
            decimal amount,
            string currency,
            Dictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100), // Convert to cents/pence
                    Currency = currency.ToLower(),
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = metadata ?? new Dictionary<string, string>()
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);

                return new PaymentIntentResult
                {
                    Success = true,
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Status = paymentIntent.Status,
                    Amount = amount,
                    Currency = currency,
                    Metadata = paymentIntent.Metadata
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating payment intent: {Message}", ex.Message);
                return new PaymentIntentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ErrorCode = ex.StripeError?.Code
                };
            }
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

        public async Task<RefundResult> CreateRefundAsync(
            string paymentIntentId,
            decimal? amount = null,
            string? reason = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Amount = amount.HasValue ? (long)(amount.Value * 100) : null,
                    Reason = reason switch
                    {
                        "requested_by_customer" => RefundReasons.RequestedByCustomer,
                        "duplicate" => RefundReasons.Duplicate,
                        "fraudulent" => RefundReasons.Fraudulent,
                        _ => null
                    }
                };

                var service = new RefundService();
                var refund = await service.CreateAsync(options, cancellationToken: cancellationToken);

                return new RefundResult
                {
                    Success = true,
                    RefundId = refund.Id,
                    PaymentIntentId = paymentIntentId,
                    Amount = (decimal)(refund.Amount / 100m),
                    Status = refund.Status
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating refund: {Message}", ex.Message);
                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
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