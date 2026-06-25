// Infrastructure/Repositories/InMemoryPaymentRepository.cs
using ToursApp.Application.Common.Interfaces;
using ToursApp.Domain.Entities;

namespace ToursApp.Infrastructure.Repositories
{
    public class InMemoryPaymentRepository : IPaymentRepository
    {
        private readonly List<Payment> _payments = new();
        private readonly List<PaymentIntent> _paymentIntents = new();
        private readonly List<Charge> _charges = new();
        private readonly object _lock = new();

        // ✅ Payment Intent operations
        public Task<PaymentIntent?> GetByStripeIdAsync(string stripePaymentIntentId)
        {
            lock (_lock)
            {
                var paymentIntent = _paymentIntents.FirstOrDefault(p => p.StripePaymentIntentId == stripePaymentIntentId);
                return Task.FromResult(paymentIntent);
            }
        }

        public Task AddPaymentIntentAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
        {
            if (paymentIntent == null)
                throw new ArgumentNullException(nameof(paymentIntent));

            lock (_lock)
            {
                if (paymentIntent.Id == Guid.Empty)
                    paymentIntent.Id = Guid.NewGuid();
                
                _paymentIntents.Add(paymentIntent);
            }
            return Task.CompletedTask;
        }

        // ✅ Payment operations
        public Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            lock (_lock)
            {
                if (payment.Id == Guid.Empty)
                    payment.Id = Guid.NewGuid();
                
                _payments.Add(payment);
            }
            return Task.CompletedTask;
        }

        public Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId)
        {
            lock (_lock)
            {
                var payment = _payments.FirstOrDefault(p => p.PaymentIntentId == paymentIntentId);
                return Task.FromResult(payment);
            }
        }

        // ✅ Charge operations
        public Task<Payment?> GetByChargeIdAsync(string stripeChargeId)
        {
            lock (_lock)
            {
                var payment = _payments.FirstOrDefault(p => p.ChargeId == stripeChargeId);
                return Task.FromResult(payment);
            }
        }

        public Task AddChargeAsync(Charge charge)
        {
            if (charge == null)
                throw new ArgumentNullException(nameof(charge));

            lock (_lock)
            {
                if (charge.Id == Guid.Empty)
                    charge.Id = Guid.NewGuid();
                
                _charges.Add(charge);
            }
            return Task.CompletedTask;
        }

        // ✅ General operations
        public Task UpdateAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            lock (_lock)
            {
                var index = _payments.FindIndex(p => p.Id == payment.Id);
                if (index != -1)
                {
                    _payments[index] = payment;
                }
            }
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // In-memory, nothing to save
            return Task.CompletedTask;
        }

        // ✅ Additional helper methods (optional)
        public Task<Payment?> GetPaymentByIdAsync(Guid id)
        {
            lock (_lock)
            {
                var payment = _payments.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(payment);
            }
        }

        public Task<IEnumerable<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId)
        {
            lock (_lock)
            {
                var payments = _payments.Where(p => p.BookingId == bookingId).ToList();
                return Task.FromResult(payments.AsEnumerable());
            }
        }

        public Task DeletePaymentAsync(Guid id)
        {
            lock (_lock)
            {
                var payment = _payments.FirstOrDefault(p => p.Id == id);
                if (payment != null)
                {
                    _payments.Remove(payment);
                }
            }
            return Task.CompletedTask;
        }

        public Task<bool> PaymentExistsAsync(Guid id)
        {
            lock (_lock)
            {
                var exists = _payments.Any(p => p.Id == id);
                return Task.FromResult(exists);
            }
        }
    }
}