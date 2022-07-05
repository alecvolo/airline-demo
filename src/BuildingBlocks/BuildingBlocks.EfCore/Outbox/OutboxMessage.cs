using System;
using System.Text.Json;
using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.EfCore.Outbox;

public record OutboxMessage
{
    public enum States
    {
        NotPublished = 0,
        InProgress = 1,
        Published = 2,
        PublishedFailed = 3
    }
    public Guid EventId { get; init; }

    /// <summary>
    /// Event type full name.
    /// </summary>
    public string EventTypeName { get; init; } = null!;

    /// <summary>
    /// Gets the date the message occurred.
    /// </summary>
    public DateTimeOffset EventDateTime { get; init; }

    /// <summary>
    /// Gets the event data - serialized to JSON.
    /// </summary>
    public string Content { get; init; } = null!;

    /// <summary>
    /// Gets the TraceId of our event.
    /// </summary>
    public Guid? TraceId { get; init; }
    public States  State { get;  set; }


    /// <summary>
    /// Gets the date the message processed.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }


    public int TimesSent { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxMessage"/> class.
    /// Initializes a new outbox message.
    /// </summary>
    /// <param name="event">An event to publish.</param>
    /// <param name="traceId">The traceId of our outbox event.</param>
    public static  OutboxMessage Create(IIntegrationEvent @event, Guid? traceId = null)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        if (@event.EventId == Guid.Empty)
        {
            throw new InvalidOperationException("EventId should be not empty");
        }

        if (string.IsNullOrEmpty(@event.EventTypeName))
        {
            throw new InvalidOperationException("EventTypeName should be not empty");
        }
        return new OutboxMessage()
        {
            EventId = @event.EventId,
            EventTypeName = @event.EventTypeName,
            EventDateTime = @event.OccurredOn,
            Content = JsonSerializer.Serialize(@event),
            TraceId = traceId,
            State = States.NotPublished,
            UpdatedAt = DateTime.UtcNow
        };
    }
}