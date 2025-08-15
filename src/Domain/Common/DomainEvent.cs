// Domain/Common/DomainEvent.cs
namespace ToursApp.Domain.Common;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public bool IsPublished { get; set; } = false; // For event publishing tracking
}