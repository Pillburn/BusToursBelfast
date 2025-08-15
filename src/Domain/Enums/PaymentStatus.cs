// Domain/Enums/PaymentStatus.cs
namespace ToursApp.Domain.Enums;

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed,
    PartiallyRefunded,
    FullyRefunded,
    Disputed
}