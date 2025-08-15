// Infrastructure/Persistence/Repositories/PaymentRepository.cs
using Microsoft.EntityFrameworkCore;

namespace ToursApp.Infrastructure.Persistence.Repositories;

public class PaymentRepository : Application.Common.Interfaces.IPaymentRepo
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentIntent?> GetByStripeIdAsync(string stripePaymentIntentId)
        => await _context.PaymentIntents
            .FirstOrDefaultAsync(pi => pi.StripePaymentIntentId == stripePaymentIntentId);

    public async Task AddPaymentIntentAsync(PaymentIntent paymentIntent)
    {
        await _context.PaymentIntents.AddAsync(paymentIntent);
    }

    public async Task<Charge?> GetByChargeIdAsync(string stripeChargeId)
        => await _context.Charges
            .FirstOrDefaultAsync(c => c.StripeChargeId == stripeChargeId);

    public async Task AddChargeAsync(Charge charge)
    {
        await _context.Charges.AddAsync(charge);
    }

    public async Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId)
        => await _context.Payments
            .Include(p => p.PaymentIntent)
            .FirstOrDefaultAsync(p => p.PaymentIntent.StripePaymentIntentId == paymentIntentId);

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}