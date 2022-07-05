using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Middleware;

public static class JwtExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services, Action<JwtBearerOptions>? configureOptions = null)
    {
        var jwtOptions = services.GetOptions<JwtBearerOptions>("Jwt");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = jwtOptions.Authority;
                options.TokenValidationParameters.ValidateAudience = false;
                configureOptions?.Invoke(options);
            });

        if (!string.IsNullOrEmpty(jwtOptions.Audience))
        {
            services.AddAuthorization(options =>
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", jwtOptions.Audience);
                })
            );
        }

        return services;
    }
}