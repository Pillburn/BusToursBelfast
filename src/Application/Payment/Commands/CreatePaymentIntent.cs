
using MediatR;

public class CreatePaymentIntentCommand : IRequest<PaymentIntentResult>
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "gbp";
}