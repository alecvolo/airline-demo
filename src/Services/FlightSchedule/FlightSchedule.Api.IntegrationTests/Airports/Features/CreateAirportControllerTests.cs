using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.IntegrationTests.Fixtures;
using FlightSchedule.Api.IntegrationTests.Infrastructure;
using FlightSchedule.Api.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Api.IntegrationTests.Airports.Features;

[Trait("Category", "Integration")]
public class CreateAirportControllerTests : AirportControllerTestsBase
{
    public CreateAirportControllerTests(ApiWebApplicationFactory fixture) : base(fixture)
    {
    }
    [Fact()]
    public async Task Should_Create_Airport()
    {
        var command = new { IataCode = NextIataCode().ToLower(), Name = "some name" };

        var response =
            await Client.PostAsync(ApiV1AirportsUrl, command.ToAppJsonContext());
        response.StatusCode.Should().Be(HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
        var model = await response.Content.FromAppJsonAsync<AirportViewModel>();
        model!.Id.Should().NotBeEmpty();
        var airport = await CreateDbContext().GetAirportByIdAsync(model!.Id);
        airport.Should().NotBeNull();
        ((string)airport!.IataCode).Should().Be(command.IataCode.ToUpper());
        ((string)airport.Name).Should().Be(command.Name);
        var resourceLocation = response.Headers.GetValues("Location").FirstOrDefault();
        resourceLocation.Should().NotBeNull();
        new Uri(resourceLocation!).LocalPath.Should().Be($"{ApiV1AirportsUrl}/{model.Id}");
    }

    [Fact()]
    public async Task Should_Return_BadRequest_When_IataCode_Is_Duplicated()
    {
        var command = new { IataCode = NextIataCode(), Name = "some name" };

        var response =
            await Client.PostAsync(ApiV1AirportsUrl, command.ToAppJsonContext());
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var model = await response.Content.FromAppJsonAsync<AirportViewModel>();
        model!.Id.Should().NotBeEmpty();
        response =
            await Client.PostAsync(ApiV1AirportsUrl, command.ToAppJsonContext());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}