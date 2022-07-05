using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.IntegrationTests.Fixtures;
using FlightSchedule.Api.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Api.IntegrationTests.Airports.Features;

[Trait("Category", "Integration")]
public class GetAirportByCodeControllerTests : AirportControllerTestsBase
{
    public GetAirportByCodeControllerTests(ApiWebApplicationFactory fixture) : base(fixture)
    {
    }

    [Fact()]
    public async Task Should_Return_Airport()
    {
        var airport = await CreateDbContext().CreateAirportAsync(NextIataCode(), "Name");
        var result = await Client.GetFromJsonAsync<AirportViewModel>($"{ApiV1AirportsUrl}/{(string)airport.IataCode}");
        result.Should().NotBeNull();
        result!.Id.Should().Be(airport.Id);
        result.IataCode.Should().Be(airport.IataCode);
        result.Name.Should().Be(airport.Name);
    }

    [Fact()]
    public async Task Should_Return_NotFound404()
    {
        var airport = await CreateDbContext().CreateAirportAsync(NextIataCode(), "Name");
        var response = await Client.GetAsync($"{ApiV1AirportsUrl}/{NextIataCode()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    [Theory()]
    [InlineData("XX")]
    [InlineData("YYYY")]
    public async Task Should_Return_BadRequest(string code)
    {
        var response = await Client.GetAsync($"{ApiV1AirportsUrl}/{code}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}