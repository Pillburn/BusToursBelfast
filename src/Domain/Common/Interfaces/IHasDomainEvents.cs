// Domain/Common/Interfaces/IHasDomainEvents.cs
using System.Collections.Generic;

namespace ToursApp.Domain.Common.Interfaces;

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}