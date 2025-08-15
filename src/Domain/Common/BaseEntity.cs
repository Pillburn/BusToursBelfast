// Domain/Common/BaseEntity.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToursApp.Domain.Common.Interfaces;

namespace ToursApp.Domain.Common;

public abstract class BaseEntity : IHasDomainEvents
{
    private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
    private Guid _id;

    public Guid Id
    {
        get => _id;
        protected set => _id = value;
    }

    // Changed to explicitly implement the interface
    IReadOnlyCollection<DomainEvent> IHasDomainEvents.DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent eventItem)
    {
        if (eventItem == null) throw new ArgumentNullException(nameof(eventItem));
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    // Optional: Additional method not in interface
    protected ReadOnlyCollection<DomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
}