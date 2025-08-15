using MediatR;
using ToursApp.Domain.Interfaces;

public class UpdateBookingPaymentStatusHandler 
    : INotificationHandler<PaymentSucceededNotification>
{
    private readonly IBookingRepository? _bookingRepository;

    public async Task Handle(
        PaymentSucceededNotification notification, 
        CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository
            .GetByPaymentIntentIdAsync(notification.PaymentIntentId);

        booking.MarkAsPaid(notification.ChargeId, notification.Amount);
        await _bookingRepository.SaveChangesAsync();
    }
}