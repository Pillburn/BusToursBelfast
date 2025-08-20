// Domain/Common/BaseEntity.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToursApp.Domain.Common.Interfaces;

namespace ToursApp.Domain.Common;

public abstract class BaseEntity : AuditableEntity, IHasDomainEvents
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    private readonly List<DomainEvent> _domainEvents = new();
    protected BaseEntity(string createdBy)
    {
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
    }

    IReadOnlyCollection<DomainEvent> IHasDomainEvents.DomainEvents =>
    new ReadOnlyCollection<DomainEvent>(_domainEvents);


    public void AddDomainEvent(DomainEvent eventItem)
    {
        if (eventItem == null) throw new ArgumentNullException(nameof(eventItem));
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
    
    // Other common entity properties/methods
    
}