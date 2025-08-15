using MediatR;
using ToursApp.Domain.Interfaces;

public class CreatePaymentIntentHandler 
    : IRequestHandler<CreatePaymentIntentCommand, PaymentIntentResult>
{
    private readonly IStripePaymentService _stripeService;
    private readonly IApplicationDbContext _context;

    public async Task<PaymentIntentResult> Handle(
        CreatePaymentIntentCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Call Stripe
        var stripeIntent = await _stripeService.CreatePaymentIntentAsync(
            request.Amount, 
            request.Currency);
        
        // 2. Create domain entity
        var paymentIntent = new PaymentIntent(
            stripeIntent.Id,
            request.Amount,
            request.Currency,
            stripeIntent.ClientSecret)
        {
            ApplicationReference = request.BookingId.ToString()
        };
        
        // 3. Persist
        _context.PaymentIntents.Add(paymentIntent);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new PaymentIntentResult(
            paymentIntent.Id,
            paymentIntent.ClientSecret,
            paymentIntent.Status.ToString());
    }
}