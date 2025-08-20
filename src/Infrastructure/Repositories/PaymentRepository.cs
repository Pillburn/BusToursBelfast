using Microsoft.EntityFrameworkCore;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Domain.Entities;
using ToursApp.Infrastructure.Persistence;

namespace ToursApp.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    // 1. Fixed return type (PaymentIntent? instead of Payment?)
    public async Task<PaymentIntent?> GetByStripeIdAsync(string stripePaymentIntentId)
        => await _context.PaymentIntents
            .FirstOrDefaultAsync(pi => pi.StripePaymentIntentId == stripePaymentIntentId);

    // 2. Added CancellationToken parameter
    public async Task AddPaymentIntentAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
        => await _context.PaymentIntents.AddAsync(paymentIntent, cancellationToken);

    // 3. Added missing method (with CancellationToken)
    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken)
        => await _context.Payments.AddAsync(payment, cancellationToken);

    // 4. Changed return type from Charge? to Payment
    public async Task<Payment> GetByChargeIdAsync(string stripeChargeId)
        => await _context.Payments
            .Include(p => p.Charge) // Ensure Charge is included if needed
            .FirstOrDefaultAsync(p => p.Charge != null && p.Charge.StripeChargeId == stripeChargeId);

    // 5. Implemented as-is (matches interface)
    public async Task AddChargeAsync(Charge charge)
        => await _context.Charges.AddAsync(charge);

    // 6. Matches interface (Payment? return type)
    public async Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId)
        => await _context.Payments
            .Include(p => p.PaymentIntent)
            .FirstOrDefaultAsync(p => p.PaymentIntent != null && 
                                    p.PaymentIntent.StripePaymentIntentId == paymentIntentId);

    // 7. Implemented as-is
    public async Task UpdateAsync(Payment payment)
        => _context.Payments.Update(payment);

    // 8. Matches interface
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}