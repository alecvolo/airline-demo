using System;
using System.Threading.Tasks;
using Bogus;
using FlightSchedule.Api.IntegrationTests.Fixtures;
using FlightSchedule.Api.IntegrationTests.Infrastructure;
using FlightSchedule.Api.Tests.Fixtures;
using FlightSchedule.Api.Tests.Infrastructure;
using FlightSchedule.Domain;
using FlightSchedule.Domain.EfCore;
using Xunit;

namespace FlightSchedule.Api.IntegrationTests.Airports.Features;

[Trait("Category", "Integration")]
public class AirportControllerTestsBase : IntegrationTest
{
    protected Func<FlightDbContext> CreateDbContext;

    public AirportControllerTestsBase(ApiWebApplicationFactory fixture) : base(fixture)
    {
        CreateDbContext = fixture.DbContextFactory;
    }

    protected const string ApiV1AirportsUrl = "/api/v1.0/airports";
    private static readonly LettersCodeGenerator IataCodeGenerator = new LettersCodeGenerator(3);
    protected string NextIataCode() => IataCodeGenerator.Next();

    protected Faker Faker { get; } = new();

    protected async Task<Airport> CreateAirportTestInstanceAsync()
    {
        await using var dbContext = CreateDbContext();
        return await dbContext.CreateAirportAsync(NextIataCode(), Faker.Company.CompanyName(), Faker.Address.FullAddress());
    }
}