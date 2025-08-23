using MediatR;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Domain.Entities; // Add this namespace
using ToursApp.Domain.Enums;
using ToursApp.Domain.Interfaces;   // Add for PaymentIntentStatus if needed

namespace ToursApp.Application.Payments.Commands;
public class CreatePaymentIntentHandler
    : IRequestHandler<CreatePaymentIntentCommand, PaymentIntentResult>
{
    private readonly IStripePaymentService _stripeService;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService; // Inject user service

    public CreatePaymentIntentHandler(
        IStripePaymentService stripeService,
        IApplicationDbContext context,
        ICurrentUserService currentUserService) // Add parameter
    {
        _stripeService = stripeService;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PaymentIntentResult> Handle(
        CreatePaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get current user ID
        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User must be authenticated");

        // 2. Call Stripe with required createdBy parameter
        var paymentIntent = await _stripeService.CreatePaymentIntentAsync(
            request.Amount,
            request.Currency,
            currentUserId); // Add createdBy

        // 3. Set application reference if booking ID exists
        if (request.BookingId.HasValue) // Fix BookingId access
        {
            paymentIntent.SetApplicationReference(request.BookingId.Value.ToString());
        }

        // 4. Persist
        _context.PaymentIntents.Add(paymentIntent);
        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentIntentResult(
            paymentIntent.Id.ToString(),
            paymentIntent.ClientSecret ?? string.Empty,
            paymentIntent.Status.ToString());
    }
}