using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Event;
using BuildingBlocks.EfCore.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EfCore;

public class EfTxOutboxBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>, IOutboxTransactional
    where TResponse : notnull
{
    private readonly ILogger<EfTxOutboxBehavior<TRequest, TResponse>> _logger;
    private readonly DbContext _dbContextBase;
    private readonly Dictionary<Type, IDomainEventMapper<IAggregate>> _eventMappers;

    private readonly IEfOutboxService _outBoxService;

    public EfTxOutboxBehavior(
        ILogger<EfTxOutboxBehavior<TRequest, TResponse>> logger,
        DbContext dbContextBase,
        IEfOutboxService outBoxService,
        IEnumerable<IDomainEventMapper<IAggregate>> eventMappers)
    {
        _logger = logger;
        _dbContextBase = dbContextBase;
        _eventMappers = eventMappers.ToDictionary(t=>t.AggregateType, t=>t);
        _outBoxService = outBoxService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (_dbContextBase.Database.CurrentTransaction != null)
        {
            return await next();
        }
        var requestTypeName = typeof(TRequest).FullName;
        _logger.LogDebug(
            "{Prefix} Handled command {MediatrRequest}",
            nameof(EfTxOutboxBehavior<TRequest, TResponse>),
            requestTypeName);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
                nameof(EfTxOutboxBehavior<TRequest, TResponse>),
                requestTypeName,
                JsonSerializer.Serialize(request));
            _logger.LogTrace(
                "{Prefix} Open the transaction for {MediatrRequest}",
                nameof(EfTxOutboxBehavior<TRequest, TResponse>),
                requestTypeName);
        }
        var transaction = await _dbContextBase.Database.BeginTransactionAsync(cancellationToken);
        var transactionId = transaction.TransactionId;
        var domainEntities = new List<(IAggregate Aggregate, IAggregate OldAggregate, IEnumerable<IDomainEvent<IAggregate>> Events)>();
        _dbContextBase.SavingChanges += delegate(object? sender, SavingChangesEventArgs args)
        {
            domainEntities = _dbContextBase.ChangeTracker
                .Entries<IAggregate>()
                .Where(x => x.Entity.GetDomainEvents().Any())
                .Select(x => (x.Entity, (IAggregate)x.OriginalValues.ToObject(), x.Entity.GetDomainEvents()))
                .ToList();
        };

        try
        {
            var response = await next();

            _logger.LogTrace(
                "{Prefix} Executed the {MediatrRequest} request",
                nameof(EfTxOutboxBehavior<TRequest, TResponse>),
                requestTypeName);


            if (_dbContextBase.ChangeTracker.HasChanges())
            {
                await _dbContextBase.SaveChangesAsync(true, cancellationToken);
            }

            var integrationEvents = new List<IIntegrationEvent>();
            domainEntities.ForEach(t =>
            {
                if (_eventMappers.TryGetValue(t.Aggregate.GetType(), out var eventMapper)
                    || _eventMappers.TryGetValue(typeof(IAggregate), out eventMapper))
                {
                    integrationEvents.AddRange(t.Events.SelectMany(e => eventMapper.Map(t.Aggregate, t.OldAggregate, e)));
                }
                t.Aggregate.ClearDomainEvents();
            });

            //var integrationEvents = _eventMappers.Map(domainEntities);
            await _outBoxService.SaveAsync(_dbContextBase.Database, integrationEvents, transaction.TransactionId, cancellationToken);
            await _dbContextBase.Database.CommitTransactionAsync(cancellationToken);
#pragma warning disable CS4014
            _outBoxService.PublishAsync(transaction.TransactionId);
#pragma warning restore CS4014

            return response;
        }
        catch
        {
            await _dbContextBase.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}