using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Bogus;
using FlightSchedule.Api.Tests.Infrastructure;
using FlightSchedule.Domain;
using FlightSchedule.Domain.EfCore;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Tests.Fixtures;

public class RequestTestsBase 
{
    private static readonly LettersCodeGenerator IataCodeGenerator = new (3);
    protected FlightDbContext GetDbContext() => CreateDbContext(DataBaseName);
    protected string DataBaseName { get; }
    protected Faker Faker { get; } = new();

    public RequestTestsBase()
    {
        DataBaseName = Guid.NewGuid().ToString("N");
    }
    public static FlightDbContext CreateDbContext(string databaseName = "test-db")
    {
        var builder = new DbContextOptionsBuilder<FlightDbContext>();
        builder.UseInMemoryDatabase(databaseName);
        builder.EnableDetailedErrors().LogTo(message => Debug.WriteLine(message));
        return new FlightDbContext(builder.Options);
    }

    protected async Task<T> WithFlightDbContext<T>(Func<FlightDbContext, Task<T>> func)
    {
        await using var dbContext = GetDbContext();
        return await func(dbContext);
    }
    protected async Task WithFlightDbContext(Func<FlightDbContext, Task> action)
    {
        await using var dbContext = GetDbContext();
        await action(dbContext);
    }
    protected async Task<Airport> CreateAirportTestInstanceAsync()
    {
        await using var dbContext = CreateDbContext();
        return await dbContext.CreateAirportAsync(NextIataCode(), Faker.Company.CompanyName(), Faker.Address.FullAddress());
    }
    protected string NextIataCode() => IataCodeGenerator.Next();

}