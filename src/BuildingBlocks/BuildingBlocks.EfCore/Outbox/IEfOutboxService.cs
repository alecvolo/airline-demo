using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Event;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.EfCore.Outbox;

public interface IEfOutboxService
{
    Task SaveAsync(DatabaseFacade database, IEnumerable<IIntegrationEvent> integrationEvent, Guid? traceId = null,
        CancellationToken cancellationToken = default);
    Task PublishAsync(Guid traceId, CancellationToken cancellationToken = default);
}