using System;

namespace BuildingBlocks.Domain.Event;

public interface IEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }
    string EventTypeName { get; }
    string DefaultEventTypeName => GetType().FullName ?? GetType().Name;

}

