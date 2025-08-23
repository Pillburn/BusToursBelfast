
using MediatR;

namespace ToursApp.Application.Payments.Commands;
public record CreatePaymentIntentCommand(
    decimal Amount,
    string Currency,
    Guid? BookingId = null) : IRequest<PaymentIntentResult>;