using ToursApp.Domain.Enums;

namespace ToursApp.Application.Common.Interfaces;
public interface IStripeEventMapper
{
    PaymentEventType MapToDomainEvent(string stripeEventType);
}