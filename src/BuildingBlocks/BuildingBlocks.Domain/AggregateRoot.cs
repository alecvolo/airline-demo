using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Domain;

public interface IAggregate
{
    public IEnumerable<IDomainEvent<IAggregate>> GetDomainEvents();
    public void ClearDomainEvents();
}
public abstract class AggregateRoot<T, TId> : IAggregate
{
    public TId Id { get; protected set; }
    public int Version { get; private set; } = -1;

    protected abstract void When(IDomainEvent<T> @event);

    private readonly List<IDomainEvent<T>> _changes = new ();

    protected AggregateRoot(){}

    protected void Apply(IDomainEvent<T> @event)
    {
        When(@event);
        EnsureValidState();
        _changes.Add(@event);
        Version++;
    }

    protected AggregateRoot<T, TId> With(Action? action = null)
    {
        action?.Invoke();
        return this;
    }

    public void Load(IEnumerable<IDomainEvent<T>> history)
    {
        foreach (var e in history)
        {
            When(e);
            Version++;
        }
    }

    //protected void AddDomainEvent(IDomainEvent<T> @event) => _changes.Add(@event);

    protected abstract void EnsureValidState();

    public IEnumerable<IDomainEvent<IAggregate>> GetDomainEvents() => (IEnumerable<IDomainEvent<IAggregate>>)_changes.AsEnumerable();
    public void ClearDomainEvents() => _changes.Clear();
}