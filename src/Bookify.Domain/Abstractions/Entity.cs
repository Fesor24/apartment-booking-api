﻿namespace Bookify.Domain.Abstractions;
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    protected Entity(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; private set; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents;
    public void ClearDomainEvents() => _domainEvents.Clear();
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
