// Infrastructure/Services/FakeStripePaymentService.cs
using ToursApp.Application.Common.Interfaces;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Enums;
using ToursApp.Domain.Interfaces;

namespace ToursApp.Infrastructure.Services
{
    public class FakeStripePaymentService : IStripePaymentService
    {
        private readonly Dictionary<string, PaymentIntent> _paymentIntents = new();
        private readonly Dictionary<string, Payment> _payments = new();
        private readonly List<Payment> _paymentHistory = new();
        private readonly object _lock = new();

        public Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency, string createdBy)
        {
            lock (_lock)
            {
                var paymentIntent = new PaymentIntent(
                    stripePaymentIntentId: "pi_fake_" + Guid.NewGuid().ToString().Substring(0, 8),
                    amount: amount,
                    currency: currency,
                    clientSecret: "secret_fake_" + Guid.NewGuid().ToString().Substring(0, 8),
                    createdBy: createdBy,
                    status: PaymentIntentStatus.RequiresPaymentMethod
                )
                {
                    Id = Guid.NewGuid(),
                    Status = PaymentIntentStatus.RequiresPaymentMethod
                };

                _paymentIntents[paymentIntent.StripePaymentIntentId] = paymentIntent;
                
                return Task.FromResult(paymentIntent);
            }
        }

        public Task<Payment> ProcessPaymentAsync(string paymentIntentId)
        {
            lock (_lock)
            {
                // Find the payment intent
                if (!_paymentIntents.TryGetValue(paymentIntentId, out var paymentIntent))
                {
                    throw new KeyNotFoundException($"Payment intent {paymentIntentId} not found");
                }

                // Update payment intent status
                paymentIntent.Status = PaymentIntentStatus.Succeeded;

                // Create a payment record
                var payment = new Payment(
                    amount: paymentIntent.Amount,
                    currency: paymentIntent.Currency,
                    createdBy: "system"
                )
                {
                    Id = Guid.NewGuid(),
                    PaymentIntentId = paymentIntent.StripePaymentIntentId,
                    Status = PaymentStatus.Succeeded,
                    PaidAt = DateTime.UtcNow
                };

                // Create a charge
                var charge = new Charge
                {
                    Id = Guid.NewGuid(),
                    ChargeId = "charge_" + Guid.NewGuid().ToString().Substring(0, 8),
                    StripeChargeId = "ch_fake_" + Guid.NewGuid().ToString().Substring(0, 8),
                    StripePaymentIntentId = paymentIntent.StripePaymentIntentId,
                    Amount = paymentIntent.Amount,
                    Currency = paymentIntent.Currency,
                    Status = ChargeStatus.Succeeded,
                    CreatedAt = DateTime.UtcNow,
                    PaymentId = payment.Id
                };

                payment.ChargeId = charge.StripeChargeId;
                _payments[paymentIntentId] = payment;
                _paymentHistory.Add(payment);

                return Task.FromResult(payment);
            }
        }

        public Task HandleWebhookEventAsync(string json, string stripeSignature)
        {
            lock (_lock)
            {
                // Simulate webhook processing
                // In a real implementation, you would:
                // 1. Verify the signature
                // 2. Parse the JSON
                // 3. Handle the event based on type

                // For demo purposes, we'll just log and return
                Console.WriteLine($"Fake webhook received: {json?.Substring(0, Math.Min(100, json?.Length ?? 0))}...");
                
                // Simulate processing a payment.succeeded event
                // In a real webhook, you'd extract the payment intent ID and update status
                
                return Task.CompletedTask;
            }
        }

        // Additional helper methods for testing
        public Task<PaymentIntent?> GetPaymentIntentByStripeIdAsync(string stripePaymentIntentId)
        {
            lock (_lock)
            {
                _paymentIntents.TryGetValue(stripePaymentIntentId, out var paymentIntent);
                return Task.FromResult(paymentIntent);
            }
        }

        public Task<Payment?> GetPaymentByIntentIdAsync(string paymentIntentId)
        {
            lock (_lock)
            {
                _payments.TryGetValue(paymentIntentId, out var payment);
                return Task.FromResult(payment);
            }
        }

        public Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_paymentHistory.AsEnumerable());
            }
        }

        public Task<int> GetPaymentCountAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_paymentHistory.Count);
            }
        }

        public Task ResetAsync()
        {
            lock (_lock)
            {
                _paymentIntents.Clear();
                _payments.Clear();
                _paymentHistory.Clear();
            }
            return Task.CompletedTask;
        }
    }
}