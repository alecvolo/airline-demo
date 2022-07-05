using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Domain.Event;

public interface IEventMapper
{
    IEnumerable<IIntegrationEvent> Map(IDomainEvent @event);
    IEnumerable<IIntegrationEvent> Map(IEnumerable<IDomainEvent> events) => events.SelectMany(Map);
}

public class DefaultEventMapper : IEventMapper
{
    public IEnumerable<IIntegrationEvent> Map(IDomainEvent @event) => Enumerable.Empty<IIntegrationEvent>();
}