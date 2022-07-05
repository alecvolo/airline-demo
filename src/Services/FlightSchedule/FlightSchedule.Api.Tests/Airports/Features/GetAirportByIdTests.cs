using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightSchedule.Api.Airports.Features;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Api.Tests.Airports.Features;

public class GetAirportByIdTests : RequestTestsBase
{
    public GetAirportByIdTests()
    {
    }

    [Fact()]
    public async Task Should_Get_Airport_By_Id()
    {
        await using var dbContext = GetDbContext();
        var airport1 = await dbContext.CreateAirportAsync("xxx", "some name for xxx");
        var airport2 = await dbContext.CreateAirportAsync("yyy", "some name for yyy");

        var handler = new GetAirportById.Handler(GetDbContext());
        var viewModel = await handler.Handle(new GetAirportById.Query(airport1.Id), CancellationToken.None);
        viewModel.Should().NotBeNull();
        viewModel.Should().BeEquivalentTo(new AirportViewModel
            { Id = airport1.Id, Address = airport1.Address, IataCode = airport1.IataCode, Name = airport1.Name });
        viewModel = await handler.Handle(new GetAirportById.Query(airport2.Id), CancellationToken.None);
        viewModel.Should().NotBeNull();
        viewModel.Should().BeEquivalentTo(new AirportViewModel
            { Id = airport2.Id, Address = airport2.Address, IataCode = airport2.IataCode, Name = airport2.Name });
    }
    [Fact()]
    public async Task Should_Return_Null()
    {
        var airport = await WithFlightDbContext(dbContext=>dbContext.CreateAirportAsync("xxx", "some name"));

        GetDbContext().Airports.Any().Should().BeTrue();
        var command = new GetAirportById.Query(Guid.NewGuid());
        var handler = new GetAirportById.Handler(GetDbContext());
        var viewModel = await handler.Handle(command, CancellationToken.None);
        viewModel.Should().BeNull();
    }
}