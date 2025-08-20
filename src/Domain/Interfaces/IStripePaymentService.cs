using System;
using ToursApp.Domain.Entities;

namespace ToursApp.Domain.Interfaces;
public interface IStripePaymentService
{
    Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency,string createdBy);
    Task<Payment> ProcessPaymentAsync(string paymentIntentId);
    Task HandleWebhookEventAsync(string json, string stripeSignature);
}