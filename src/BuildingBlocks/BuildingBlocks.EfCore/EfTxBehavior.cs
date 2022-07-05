using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EfCore;

public class EfTxBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>, ITransactional
    where TResponse : notnull
{
    private readonly ILogger<EfTxBehavior<TRequest, TResponse>> _logger;
    private readonly DbContext _dbContextBase;

    public EfTxBehavior(
        ILogger<EfTxBehavior<TRequest, TResponse>> logger,
        DbContext dbContextBase)
    {
        _logger = logger;
        _dbContextBase = dbContextBase;
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
            nameof(EfTxBehavior<TRequest, TResponse>),
            requestTypeName);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
                nameof(EfTxBehavior<TRequest, TResponse>),
                requestTypeName,
                JsonSerializer.Serialize(request));
            _logger.LogTrace(
                "{Prefix} Open the transaction for {MediatrRequest}",
                nameof(EfTxBehavior<TRequest, TResponse>),
                requestTypeName);
        }
        await using var transaction = await _dbContextBase.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            _logger.LogTrace(
                "{Prefix} Executed the {MediatrRequest} request",
                nameof(EfTxBehavior<TRequest, TResponse>),
                requestTypeName);


            if (_dbContextBase.ChangeTracker.HasChanges())
            {
                await _dbContextBase.SaveChangesAsync(true, cancellationToken);
            }

            await _dbContextBase.Database.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch
        {
            await _dbContextBase.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

}