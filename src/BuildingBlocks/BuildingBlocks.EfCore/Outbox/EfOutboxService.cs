using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EfCore.Outbox;

public class EfOutboxService : IEfOutboxService, IOutboxService
{
    private readonly Func<DbConnection?, OutboxDbContext> _createOutboxContextFactory;
    private readonly IOutboxMessagePublisher _publisher;
    private readonly ILogger<EfOutboxService> _logger;
    public static Expression<Func<OutboxMessage, bool>> TruePredicate = (_) => true;

    public EfOutboxService(Func<DbConnection?, OutboxDbContext> createOutboxContextFactory, IOutboxMessagePublisher publisher, ILogger<EfOutboxService> logger)
    {
        _createOutboxContextFactory = createOutboxContextFactory ?? throw new ArgumentNullException(nameof(createOutboxContextFactory));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task SaveAsync(DatabaseFacade database, IEnumerable<IIntegrationEvent> integrationEvents,
        Guid? traceId = null,
        CancellationToken cancellationToken = default)
    {
        if (database == null) throw new ArgumentNullException(nameof(database));
        if (integrationEvents == null) throw new ArgumentNullException(nameof(integrationEvents));
        var transaction = database.CurrentTransaction;
        if (transaction == null) throw new InvalidOperationException("Transaction has not started yet");
        traceId ??= transaction.TransactionId;
        await using var dbContext = _createOutboxContextFactory(database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        await dbContext.OutboxMessages.AddRangeAsync(integrationEvents.Where(e=>e!=null).Select(e => OutboxMessage.Create(e, traceId)), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }


    public async Task ProcessUnprocessed(Expression<Func<OutboxMessage, bool>>? predicate = null, int limit = 100,
        CancellationToken cancellationToken = default)
    {
        predicate ??= TruePredicate;
        limit  = limit < 1 ? 1 : limit;
        List<OutboxMessage> messages;
        {
            await using var dbContext = _createOutboxContextFactory(null);
            messages = await dbContext.OutboxMessages.Where(t => t.State == OutboxMessage.States.NotPublished)
                .Where(predicate)
                //.OrderBy(t => t.EventDateTime)
                .Take(limit)
                .AsNoTracking().ToListAsync(cancellationToken);
        }
        foreach (var message in messages)
        {
            try
            {
                if (await UpdateEventStatus(message.EventId, message.State, OutboxMessage.States.InProgress,
                        cancellationToken))
                {
                    await _publisher.PublishAsync(message);
                    await UpdateEventStatus(message.EventId, OutboxMessage.States.InProgress, OutboxMessage.States.Published, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Publishing event #{EventId} failed", message.EventId);
                await UpdateEventStatus(message.EventId, OutboxMessage.States.InProgress, OutboxMessage.States.PublishedFailed, cancellationToken);
            }
        }
    }

    public Task PublishAsync(Guid traceId, CancellationToken cancellationToken = default)
    {
        var task = Task.Factory.StartNew(async () =>
        {
            await ProcessUnprocessed(t => t.TraceId == traceId, 20, cancellationToken);
        }, TaskCreationOptions.LongRunning);
        return task;
    }

    private async Task<bool> UpdateEventStatus(Guid eventId, OutboxMessage.States currentStatus, OutboxMessage.States status, CancellationToken cancellationToken)
    {
        await using var dbContext = _createOutboxContextFactory(null);
        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            var eventLogEntry =
                await dbContext.OutboxMessages.FirstOrDefaultAsync(
                    ie => ie.EventId == eventId && ie.State == currentStatus,
                    cancellationToken);
            if (eventLogEntry == null)
            {
                return false;
            }

            eventLogEntry.State = status;
            eventLogEntry.UpdatedAt = DateTime.UtcNow;
            if (status == OutboxMessage.States.InProgress)
                eventLogEntry.TimesSent++;

            dbContext.OutboxMessages.Update(eventLogEntry);

            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        });


    }


    public Task ProcessUnprocessed(CancellationToken cancellation = default) =>
        ProcessUnprocessed(null, 100, cancellation);
}