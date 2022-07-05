using System;

namespace BuildingBlocks.Domain.Event;

public interface IDomainEvent: IEvent
{
    /// <summary>
    ///     Gets the name of the event.
    /// </summary>
    string DisplayName => this.GetType().Name;
}

public interface IDomainEvent<out T> : IDomainEvent
{
}

public abstract record AbstractEvent: IEvent
{
    public Guid EventId { get; protected set; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; protected set; } = DateTimeOffset.Now;
    public string EventTypeName { get; protected set; }

    protected AbstractEvent()
    {
        EventTypeName = ((IEvent)this).DefaultEventTypeName;


    }
}

