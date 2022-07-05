using System.Threading;
using System.Threading.Tasks;
using FlightSchedule.Api.Airports.Features;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.Airports.Validators;
using FlightSchedule.Api.Tests.Fixtures;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace FlightSchedule.Api.Tests.Airports.Features;

public class UpdateAirportTests : RequestTestsBase
{
    [Fact()]
    public async Task Should_Update_Airport()
    {
        var airport = await WithFlightDbContext( dbContext=> dbContext.CreateAirportAsync("xxx", "some name1"));
        var model = new AirportUpdateModel() { IataCode = "yyy", Name = "some name2" };
        var command = new UpdateAirport.Command(airport.Id, model );
        (await new UpdateAirportModelValidator().TestValidateAsync(model)).ShouldNotHaveAnyValidationErrors();
        var handler = new UpdateAirport.Handler(GetDbContext(), AirportMappingsTests.CreateMapperConfiguration().CreateMapper());
        var result = await handler.Handle(command, CancellationToken.None);
        var airport2 = await WithFlightDbContext(dbContext => dbContext.GetAirportByIdAsync(airport.Id));
        airport2.Should().NotBeNull();
        result.Should().BeEquivalentTo(new AirportViewModel
            { Id = airport2!.Id, Address = airport2.Address, IataCode = airport2.IataCode, Name = airport2.Name });
    }
}