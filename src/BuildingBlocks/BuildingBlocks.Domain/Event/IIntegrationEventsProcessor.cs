using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Domain.Event;

public interface IIntegrationEventsProcessor
{
    Task PublishAsync(IEnumerable<IIntegrationEvent> events, CancellationToken cancellationToken = default);
    Task ProcessAsync(Guid transactionId);
}