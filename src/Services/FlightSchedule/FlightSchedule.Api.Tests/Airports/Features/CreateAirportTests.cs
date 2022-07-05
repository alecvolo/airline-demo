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

public class CreateAirportTests: RequestTestsBase
{
    [Fact()]
    public async Task Should_Create_Airport()
    {
        var command = new CreateAirport.Command(new AirportUpdateModel(){IataCode = "xxx", Name = "some name"});
        (await new UpdateAirportModelValidator().TestValidateAsync(command.Model)).ShouldNotHaveAnyValidationErrors();
        var handler = new CreateAirport.Handler(GetDbContext(), AirportMappingsTests.CreateMapperConfiguration().CreateMapper());
        var model = await handler.Handle(command, CancellationToken.None);
        var airport = await WithFlightDbContext(dbContext=> dbContext.GetAirportByIdAsync(model.Id));
        await WithFlightDbContext(async dbContext =>
        {
            await dbContext.SaveChangesAsync();
        });
        airport.Should().NotBeNull();
        command.Model.IataCode.ToUpper().Should().Be(airport!.IataCode);
        command.Model.Name.Should().Be(airport.Name);
        command.Model.Address.Should().Be(airport.Address);

    }
}