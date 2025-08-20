// Domain/Common/Interfaces/IHasDomainEvents.cs
using ToursApp.Domain.Common;

namespace ToursApp.Domain.Common.Interfaces;

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void AddDomainEvent(DomainEvent eventItem);
    void ClearDomainEvents();
}