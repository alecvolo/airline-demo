using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using BuildingBlocks.EfCore;
using BuildingBlocks.Web.Middleware;
using BuildingBlocks.Web.Validators;
using EntityFramework.Exceptions.SqlServer;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.Infrastructure;
using FlightSchedule.Domain.EfCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
//
builder.Services.AddSingleton( (_) =>
{
    https://damienbod.com/2020/09/24/securing-azure-functions-using-azure-ad-jwt-bearer-token-authentication-for-user-access-tokens/
    var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        "https://login.microsoftonline.com/d25211e5-61a4-44e8-84cc-870227ea5dd8/v2.0/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());

    var openIdConfiguration =  configurationManager.GetConfigurationAsync().Result;
    return openIdConfiguration;
});
builder.Services.AddDbContext<FlightDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql"))
        .EnableDetailedErrors().UseExceptionProcessor();
});
//builder.Services.AddScoped(s =>
//{
//    var contextOptionsBuilder = new DbContextOptionsBuilder<FlightDbContext>()
//        .UseSqlServer(builder.Configuration.GetConnectionString("sql")).EnableDetailedErrors();
//    return contextOptionsBuilder.Options;
//});

builder.Services.AddVersionedApiExplorer(o =>
{
    o.GroupNameFormat = "'v'VVV";
    o.SubstituteApiVersionInUrl = true;
});
builder.Services.AddApiVersioning(
    //https://www.meziantou.net/versioning-an-asp-net-core-api.htm
    //https://exceptionnotfound.net/overview-of-api-versioning-in-asp-net-core-3-0/
    options =>
    {
        // Add the headers "api-supported-versions" and "api-deprecated-versions"
        // This is better for discoverability
        options.ReportApiVersions = true;

        // AssumeDefaultVersionWhenUnspecified should only be enabled when supporting legacy services that did not previously
        // support API versioning. Forcing existing clients to specify an explicit API version for an
        // existing service introduces a breaking change. Conceptually, clients in this situation are
        // bound to some API version of a service, but they don't know what it is and never explicit request it.
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);

        // // Defines how an API version is read from the current HTTP request
        options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"),
            new UrlSegmentApiVersionReader());
    }
);

builder.Services.AddSingleton<ITenantDbConnectionStringManager, TenantDbConnectionStringManager>();

builder.Services.AddScoped<FlightDbContext>();
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<FlightDbContext>());
builder.Services.AddScoped<IValidator<JsonPatchDocument<AirportUpdateModel>>, JsonPatchDocumentValidator<AirportUpdateModel>>();
//builder.Services.AddTransient<IValidator<CreateAirport.Command>, UpdateAirportModelValidator>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.Configure<JsonOptions>(options => options.AllowInputFormatterExceptionMessages = true);
var validators = AssemblyScanner.FindValidatorsInAssembly(typeof(Program).Assembly);


// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddMicrosoftIdentityWebApi(options =>
    //{
    //    options.
    //    options.Events.O
    //})
    //.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
    .AddJwtBearer(o  =>
    {
        o.Audience = "api://514a521d-3d22-4f57-8516-4ff4e2fe0787";
        o.Authority = "https://login.microsoftonline.com/d25211e5-61a4-44e8-84cc-870227ea5dd8";

        // we don't need code below if we set   Authority      
        //https://devblogs.microsoft.com/dotnet/jwt-validation-and-authorization-in-asp-net-core/

       // var openIdConfiguration = builder.Services.BuildServiceProvider().GetRequiredService<OpenIdConnectConfiguration>();
       // ///X509Certificate2 cert = new X509Certificate2("MySelfSignedCertificate.pfx", "password");
       //// SecurityKey key = new X509SecurityKey(cert); //well, seems to be that simple

       // o.TokenValidationParameters = new TokenValidationParameters()
       // {
       //     RequireSignedTokens = true,
       //     ValidAudience = o.Audience,
       //     ValidateAudience = true,
       //     ValidateIssuer = true,
       //     ValidateIssuerSigningKey = true,
       //     ValidateLifetime = true,
       //     IssuerSigningKeys = openIdConfiguration.SigningKeys,
       //     ValidIssuer = openIdConfiguration.Issuer
       //     //IssuerSigningKeys = openIdConfiguration.SigningKeys,
       //     //ValidIssuer = openIdConfiguration.Issuer
       // };
       o.Events = new JwtBearerEvents()
       {
           OnTokenValidated =  ctx =>
           {
               if (ctx.Principal != null)
               {
                   var oid = ctx.Principal.FindFirst("oid")?.Value;
                   var sub = ctx.Principal.FindFirst("sub")?.Value;
                   var isAppOnly = oid != null && sub != null && oid == sub;

               }
               return Task.CompletedTask;
           }
       };
    });
//    //https://stackoverflow.com/questions/46938248/asp-net-core-2-0-combining-cookies-and-bearer-authorization-for-the-same-endpoin
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//    {
//        options.Events = new CookieAuthenticationEvents()
//        {
//            OnValidatePrincipal = 
//        }
//    });
//https://stackoverflow.com/questions/55950277/how-do-i-add-a-custom-claim-to-authentication-cookie-generated-by-openidconnect
//    .AddOpenIdConnect("oidc", options =>
//{
//    options.Events = new OpenIdConnectEvents
//    {
//        OnTokenValidated = async ctx =>
//        {
//            var claim = new Claim("your-claim", "your-value");

//            var identity = new ClaimsIdentity(new[] { claim });

//            ctx.Principal.AddIdentity(identity);

//            await Task.CompletedTask;
//        }
//    };
//}
builder.Services.AddScoped<IClaimsTransformation, AppRolesToClaimsTransformation>();
builder.Services.AddControllers(o =>
{
    o.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
}).AddFluentValidation().AddNewtonsoftJson();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
builder.Services.AddAutoMapper(typeof(Program).GetTypeInfo().Assembly);
builder.Services.AddApiProblemDetails();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(SwashbuckleSchemaHelper.GetSchemaId);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Scheduled Flight Api", Version = "v1" });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl =
                    new Uri(
                        "https://login.microsoftonline.com/d25211e5-61a4-44e8-84cc-870227ea5dd8/oauth2/v2.0/authorize"), //tenant id
                TokenUrl = new Uri(
                    "https://login.microsoftonline.com/d25211e5-61a4-44e8-84cc-870227ea5dd8/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "api://514a521d-3d22-4f57-8516-4ff4e2fe0787/Write", "Write schedule" },
                    { "api://514a521d-3d22-4f57-8516-4ff4e2fe0787/Read", "Read schedule" }
                }
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scheduled Flight Api v1");
        //c.RoutePrefix = string.Empty;    
        c.OAuthClientId("514a521d-3d22-4f57-8516-4ff4e2fe0787");
       // c.OAuthClientSecret("RUv8Q~4QHz_xv8cBJE.-~w6pOyyj.XuSZ4qrqaLY");
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();

    });
}

app.UseSerilogRequestLogging(o => {
    o.EnrichDiagnosticContext = (context, httpContext) => {
        context.Set("ActivityId", Activity.Current?.Id);
    };
    o.MessageTemplate = "{ActivityId} HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    o.GetLevel = (context, duration, ex) => {
        if (ex != null || context.Response.StatusCode > 499)
            return LogEventLevel.Error;

        if (context.Response.StatusCode > 399)
            return LogEventLevel.Information;

        if (duration < 1000 || context.Request.Path.StartsWithSegments("/api/v2/push"))
            return LogEventLevel.Debug;

        return LogEventLevel.Information;
    };
});

app.UseProblemDetails();
app.UseHttpsRedirection();

app.UseRouting();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["some"] = "some";
        return Task.CompletedTask;
    });
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(conf =>
{
    conf.MapControllers();
});
//app.MapControllers();

var dbConfigurationOption = app.Services.GetRequiredService<ITenantDbConnectionStringManager>();
var vasaConnectionString = dbConfigurationOption.Get("vasa");
//net stop hns
//net start hns
//net stop winnat
//net start winnat

//Restart-Service hns ???
//https://stackoverflow.com/questions/15619921/an-attempt-was-made-to-access-a-socket-in-a-way-forbidden-by-its-access-permissi
//https://stackoverflow.com/questions/57316744/docker-sql-bind-an-attempt-was-made-to-access-a-socket-in-a-way-forbidden-by-it

app.Run();

public partial class Program {}

public class AppRolesToClaimsTransformation : IClaimsTransformation
{
    //https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-verification-scope-app-roles?tabs=aspnetcore

    private readonly Dictionary<string, string> _rolesToClams = new ()
    {
        { "FlightApi.Write", "Write"},
        { "FlightApi.Read", "Read" }
    };

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var oid = principal.FindFirst("oid")?.Value;
        var sub = principal.FindFirst("sub")?.Value;
        var isAppOnly = oid != null && sub != null && oid == sub;

        var scope = string.Join(' ',
            principal.Claims.Where(t => t.Type == ClaimTypes.Role)
                .Select(t =>_rolesToClams.TryGetValue(t.Value, out var claim)? claim : null).Where(t=>t!=null));
        if (!string.IsNullOrWhiteSpace(scope))
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimConstants.Scope, scope));
            principal.AddIdentity(identity);
            //((System.Security.Claims.ClaimsIdentity)principal.Identity).NameClaimType
            // Clone current identity
//        var clone = principal.Clone();
            //      var newIdentity = (ClaimsIdentity)clone.Identity!;
            //    newIdentity.AddClaim(new Claim("ProjectReader", "true"));
        }

        return Task.FromResult(principal);
    }
}

internal static class SwashbuckleSchemaHelper
{
    //https://stackoverflow.com/questions/61881770/invalidoperationexception-cant-use-schemaid-the-same-schemaid-is-already-us/72677918#72677918
    private static readonly Dictionary<string, int> SchemaNameRepetition = new Dictionary<string, int>();

    public static string GetSchemaId(Type type)
    {
        var id = type.Name;

        if (!SchemaNameRepetition.ContainsKey(id))
            SchemaNameRepetition.Add(id, 0);

        var count = (SchemaNameRepetition[id] + 1);
        SchemaNameRepetition[id] = count;

        return type.Name + (count > 1 ? count.ToString() : "");
    }
}

/// <summary>
/// An implementation of <see cref="IDisplayMetadataProvider"/> and <see cref="IValidationMetadataProvider"/> for
/// the System.Text.Json.Serialization attribute classes.
/// </summary>
public sealed class SystemTextJsonValidationMetadataProvider : IDisplayMetadataProvider, IValidationMetadataProvider
{
    private readonly JsonNamingPolicy _jsonNamingPolicy;

    /// <summary>
    /// Creates a new <see cref="SystemTextJsonValidationMetadataProvider"/> with the default <see cref="JsonNamingPolicy.CamelCase"/>
    /// </summary>
    public SystemTextJsonValidationMetadataProvider()
        : this(JsonNamingPolicy.CamelCase)
    { }

    /// <summary>
    /// Creates a new <see cref="SystemTextJsonValidationMetadataProvider"/> with an optional <see cref="JsonNamingPolicy"/>
    /// </summary>
    /// <param name="namingPolicy">The <see cref="JsonNamingPolicy"/> to be used to configure the metadata provider.</param>
    public SystemTextJsonValidationMetadataProvider(JsonNamingPolicy namingPolicy)
    {
        if (namingPolicy == null)
        {
            throw new ArgumentNullException(nameof(namingPolicy));
        }

        _jsonNamingPolicy = namingPolicy;
    }

    /// <inheritdoc />
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var propertyName = ReadPropertyNameFrom(context.Attributes);

        if (!string.IsNullOrEmpty(propertyName))
        {
            context.DisplayMetadata.DisplayName = () => propertyName;
        }
    }

    /// <inheritdoc />
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var propertyName = ReadPropertyNameFrom(context.Attributes);

        if (string.IsNullOrEmpty(propertyName))
        {
            propertyName = _jsonNamingPolicy.ConvertName(context.Key.Name!);
        }

//        context.ValidationMetadata. ValidationMetadata. ValidationModelName = propertyName;
    }

    private static string? ReadPropertyNameFrom(IReadOnlyList<object> attributes)
        => attributes?.OfType<JsonPropertyNameAttribute>().FirstOrDefault()?.Name;
}
