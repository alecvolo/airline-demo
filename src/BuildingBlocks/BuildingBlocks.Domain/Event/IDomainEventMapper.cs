using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Domain.Event;

public interface IDomainEventMapper<in T> where T : IAggregate
{
    IEnumerable<IIntegrationEvent?> Project(T aggregate, T originalAggregate, IDomainEvent<T> @event);
    public IEnumerable<IIntegrationEvent> Map(T aggregate, T originalAggregate, IDomainEvent<T> @event) =>
        Project(aggregate, originalAggregate, @event).Where(t=>t!=null)!;
    public Type AggregateType => typeof(T);
}

