using System.Collections.Generic;

namespace BuildingBlocks.Domain.Event;

public class AggregateDomainEvent<T> where T : IAggregate
{
    public T Aggregate { get; }
    public T OldAggregate { get; }
    public IEnumerable<IDomainEvent<T>> DomainEvents { get; }

    public AggregateDomainEvent(T aggregate, T oldAggregate, IEnumerable<IDomainEvent<T>> domainEvents)
    {
        Aggregate = aggregate;
        OldAggregate = oldAggregate;
        DomainEvents = domainEvents;
    }
}