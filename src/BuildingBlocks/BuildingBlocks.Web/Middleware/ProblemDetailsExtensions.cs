using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using BuildingBlocks.Web.Exceptions;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Web.Middleware;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddApiProblemDetails(this IServiceCollection services,
        Action<ProblemDetailsOptions>? configure = null)
    {
        return services.AddProblemDetails(options =>
        {
            configure?.Invoke(options);
            options.IncludeExceptionDetails = (context, ex) =>
            {
                if (ex is ApiException || ex is ValidationException)
                {
                    return false;
                }

                var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };
            options.OnBeforeWriteDetails = (ctx, problem) =>
            {
                problem.Extensions[options.TraceIdPropertyName] = Activity.Current?.Id ?? ctx.TraceIdentifier;
            };
            //https://restfulapi.net/http-status-codes/
            options.Map<ResourceNotFoundException>(ex => new ProblemDetails()
            {
                Type = @"https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = string.IsNullOrWhiteSpace(ex.Message) ? "Not Found" : ex.Message,
                Status = StatusCodes.Status404NotFound,
            });
            options.Map<DuplicateResourceException>(ex => new ProblemDetails()
            {
                Type = @"https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
            });
            options.Map<ResourceForbiddenException>(ex => new ProblemDetails()
            {
                Type = @"https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = string.IsNullOrWhiteSpace(ex.Message) ? "Forbidden" : ex.Message,
                Status = StatusCodes.Status403Forbidden
            });
            options.Map<ValidationException>(ex => new ValidationProblemDetails(
                    ex.Errors.GroupBy(t => t.PropertyName, (propertyName, errors) => new
                        {
                            propertyName,
                            Errors = errors.Select(t => t.ErrorMessage).ToArray()
                        }, StringComparer.CurrentCultureIgnoreCase)
                        .ToDictionary(t => JsonNamingPolicy.CamelCase.ConvertName(t.propertyName ?? string.Empty),
                            t => t.Errors, StringComparer.CurrentCultureIgnoreCase))
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Status = StatusCodes.Status400BadRequest
                }
            );
        });

    }
}