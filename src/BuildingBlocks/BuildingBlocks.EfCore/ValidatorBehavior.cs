using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EfCore;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "{Prefix} Validating command {MediatrRequest} with content {RequestContent}",
                nameof(ValidatorBehavior<TRequest, TResponse>),
                typeof(TRequest).FullName,
                JsonSerializer.Serialize(request));
        }


        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (!failures.Any())
        {
            return await next();
        }
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "{Prefix} Validation failed {MediatrRequest} with content {RequestContent}. Errors: {@ValidationErrors}",
                nameof(ValidatorBehavior<TRequest, TResponse>),
                typeof(TRequest).FullName,
                JsonSerializer.Serialize(request), failures);
        }

        throw new ValidationException(failures);

    }
}