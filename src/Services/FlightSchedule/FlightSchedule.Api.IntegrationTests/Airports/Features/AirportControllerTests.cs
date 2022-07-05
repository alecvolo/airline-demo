using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.IntegrationTests.Fixtures;
using FlightSchedule.Api.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Api.IntegrationTests.Airports.Features
{
    [Trait("Category", "Integration")]
    public class GetAirportsControllerTest : AirportControllerTestsBase
    {
        public GetAirportsControllerTest(ApiWebApplicationFactory fixture) : base(fixture)
        {
        }


        [Fact()]
        public async Task Should_Return_Single_Airport()
        {
            var airport = await CreateDbContext().CreateAirportAsync(NextIataCode(), "Name");
            var result =
                (await Client.GetFromJsonAsync<IEnumerable<AirportViewModel>>(
                     $"{AirportControllerTestsBase.ApiV1AirportsUrl}?IataCode={(string)airport.IataCode}") ??
                 Array.Empty<AirportViewModel>()).ToArray();
            result!.Should().HaveCount(1);
            result[0].Id.Should().Be(airport.Id);
            result[0].IataCode.Should().Be(airport.IataCode);
            result[0].Name.Should().Be(airport.Name);
        }

        [Fact()]
        public async Task Should_Return_All_Airports()
        {
            var airport = await CreateDbContext().CreateAirportAsync(NextIataCode(), "Name");
            await CreateDbContext().CreateAirportAsync(NextIataCode(), "Name");
            var result = (await Client.GetFromJsonAsync<IEnumerable<AirportViewModel>>(ApiV1AirportsUrl) ??
                          Array.Empty<AirportViewModel>()).ToArray();
            result!.Should().NotBeEmpty();
            result.Should().HaveCountGreaterOrEqualTo(2);
            result.Should().Contain(t => t.Id == airport.Id);
        }
    }
}