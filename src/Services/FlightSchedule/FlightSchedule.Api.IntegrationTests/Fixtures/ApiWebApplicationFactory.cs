using System;
using System.Linq;
using EntityFramework.Exceptions.Sqlite;
using FlightSchedule.Domain.EfCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlightSchedule.Api.IntegrationTests.Fixtures;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _keepAliveConnection;
    private readonly string _connectionString = "DataSource=myshareddb;mode=memory;cache=shared";
    public IConfiguration Configuration { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("integrationsettings.json")
                .Build();

            config.AddConfiguration(Configuration);
        });
        //https://stackoverflow.com/questions/56319638/entityframeworkcore-sqlite-in-memory-db-tables-are-not-created
        _keepAliveConnection = new SqliteConnection(_connectionString);
        _keepAliveConnection.Open();


        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FlightDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContext<FlightDbContext>(options =>
            {
                    options.UseSqlite(_connectionString)
                    .EnableDetailedErrors()
                    .UseExceptionProcessor();
            });
                services
                .AddAuthentication("IntegrationTest")
                    .AddScheme<AuthenticationSchemeOptions, IntegrationTestAuthenticationHandler>(
                        "IntegrationTest",
                        options => { }
                    );
                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices
                        .GetRequiredService<FlightDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<ApiWebApplicationFactory>>();

                    try
                    {
                        SeedData(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding " +
                                            "the database with test messages. Error: {Message}",
                            ex.Message);
                    }
                }

            //services.AddMvc(options => options.Filters.Add(new AllowAnonymousFilter()));
            // services.AddTransient<IWeatherForecastConfigService, WeatherForecastConfigStub>();
        });
    }

    public Func<FlightDbContext> DbContextFactory => ()=> new(new DbContextOptionsBuilder<FlightDbContext>()
        .UseSqlite(_connectionString)
        .EnableDetailedErrors().Options);

    private void SeedData(FlightDbContext DbContext)
    {
        DbContext.Database.EnsureCreated();
    }
}