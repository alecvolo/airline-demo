using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.IntegrationTests.Fixtures;
using FlightSchedule.Api.IntegrationTests.Infrastructure;
using FlightSchedule.Api.Tests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Xunit;

namespace FlightSchedule.Api.IntegrationTests.Airports.Features;

[Trait("Category", "Integration")]
public class PathAirportControllerTests : AirportControllerTestsBase
{
    public PathAirportControllerTests(ApiWebApplicationFactory fixture) : base(fixture)
    {
    }
    [Fact()]
    public async Task Should_Update_All_Properties()
    {
        var airport = await CreateAirportTestInstanceAsync();
        var command = new Operation[]
        {
            new("replace", nameof(AirportUpdateModel.Name), null, " New Value "),
            new("replace", nameof(AirportUpdateModel.IataCode), null,  NextIataCode()),
            new("replace", nameof(AirportUpdateModel.Address), null, "Some New")
        };

        var response =
            await Client.PatchAsync($"{ApiV1AirportsUrl}/{(Guid)airport.Id}", command.ToAppJsonContext());
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var resourceLocation = response.Headers.GetValues("Location").FirstOrDefault();
        resourceLocation.Should().NotBeNull();
        new Uri(resourceLocation!).LocalPath.Should().Be($"{ApiV1AirportsUrl}/{(Guid)airport.Id}");

        var model = await response.Content.FromAppJsonAsync<AirportViewModel>();
        model!.Id.Should().NotBeEmpty();
        var updatedAirport = await CreateDbContext().GetAirportByIdAsync(model!.Id);
        updatedAirport.Should().NotBeNull();
        ((string)updatedAirport!.Name).Should().Be((command[0].value as string)?.Trim());
        ((string)updatedAirport.IataCode).Should().Be(command[1].value as string);
        updatedAirport.Address.Should().Be(command[2].value as string);
    }
    [Fact()]
    public async Task Should_Update_Name_Only()
    {
        var airport = await CreateAirportTestInstanceAsync();
        var command = new Operation<AirportUpdateModel>[]  {new() {op="replace", path = nameof(AirportUpdateModel.Name), value = " New Value "} };

        var response =
            await Client.PatchAsync($"{ApiV1AirportsUrl}/{(Guid)airport.Id}", command.ToAppJsonContext());
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var resourceLocation = response.Headers.GetValues("Location").FirstOrDefault();
        resourceLocation.Should().NotBeNull();
        new Uri(resourceLocation!).LocalPath.Should().Be($"{ApiV1AirportsUrl}/{(Guid)airport.Id}");

        var model = await response.Content.FromAppJsonAsync<AirportViewModel>();
        model!.Id.Should().NotBeEmpty();
        var updatedAirport = await CreateDbContext().GetAirportByIdAsync(model!.Id);
        updatedAirport.Should().NotBeNull();
        (updatedAirport!.IataCode).Should().Be(airport.IataCode);
        ((string)updatedAirport.Name).Should().Be((command[0]!.value as string)?.Trim());
        (updatedAirport.Address).Should().Be(airport.Address);
    }

    [Fact()]
    public async Task Should_Return_404_If_Not_Exists()
    {
        var airport = await CreateDbContext().CreateAirportAsync(NextIataCode(), "Name", "Address");
        var command = new { IataCode = NextIataCode(), Name = "new name   ", Address = "New Address" };

        var response =
            await Client.PutAsync($"{ApiV1AirportsUrl}/{Guid.NewGuid()}", command.ToAppJsonContext());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    [Fact()]
    public async Task Should_Return_BadRequest_When_IataCode_Is_Duplicated()
    {
        var airport = await CreateDbContext().CreateAirportAsync(NextIataCode(), "Name", "Address");
        var airport2 = await CreateDbContext().CreateAirportAsync(NextIataCode(), "Name", "Address");
        var command = new { IataCode = (string)airport2.IataCode, Name = "new name   ", Address = "New Address" };

        var response =
            await Client.PutAsync($"{ApiV1AirportsUrl}/{(Guid)airport.Id}", command.ToAppJsonContext());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}