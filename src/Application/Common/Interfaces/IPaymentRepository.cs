// Application/Common/Interfaces/IPaymentRepository.cs
using ToursApp.Domain.Entities;

namespace ToursApp.Application.Common.Interfaces;

public interface IPaymentRepository
{
    // Payment Intent operations
    Task<PaymentIntent?> GetByStripeIdAsync(string stripePaymentIntentId);
    Task AddPaymentIntentAsync(PaymentIntent paymentIntent);
    
    // Charge operations
    Task<Charge?> GetByChargeIdAsync(string stripeChargeId);
    Task AddChargeAsync(Charge charge);
    
    // General operations
    Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId);
    Task UpdateAsync(Payment payment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}