using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Helpers;
using FlightSchedule.Api.Flights.Features;
using FlightSchedule.Api.Flights.Models;
using FlightSchedule.Api.Flights.Validators;
using FlightSchedule.Api.Tests.Fixtures;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlightSchedule.Api.Tests.Flights.Features
{
    public class CreateFlightTests: RequestTestsBase
    {
        [Fact()]
        public async Task  Should_Create_Flight()
        {
            var arrivalAirport = await GetDbContext().CreateAirportAsync(NextIataCode(), Faker.Company.CompanyName());
            var departureAirport = await GetDbContext().CreateAirportAsync(NextIataCode(), Faker.Company.CompanyName());

            var command = new CreateFlight.Command(new UpdateFlightModel()
            {
                FlightNumber = "AA123",
                DepartureAirport = departureAirport.IataCode,
                DepartureAt = DateTimeOffset.Now.TrimToMinutes(),
                ArrivalAirport = arrivalAirport.IataCode,
                ArrivalAt = DateTimeOffset.Now.AddHours(3).TrimToMinutes()
            });
            (await new UpdateFlightModelValidator().TestValidateAsync(command.Model)).ShouldNotHaveAnyValidationErrors();
            var handler = new CreateFlight.Handler(GetDbContext(), FlightMappingsTests.CreateMapperConfiguration().CreateMapper());
            var model = await handler.Handle(command, CancellationToken.None);
            model!.FlightNumber.Should().Be(command.Model.FlightNumber.ToUpper());
            model.DepartureAirport.Should().Be(departureAirport.IataCode);
            model.DepartureAt.Should().Be(command.Model.DepartureAt);
            model.ArrivalAirport.Should().Be(arrivalAirport.IataCode);
            model.ArrivalAt.Should().Be(command.Model.ArrivalAt);
            var flight = await WithFlightDbContext(dbContext => dbContext.Flights.FirstOrDefaultAsync(t=>t.Id == new FlightId(model.Id)));
            flight.Should().NotBeNull();
            ((string)flight!.FlightNumber).Should().Be(model.FlightNumber);
            flight.DepartureAirportId.Should().Be(departureAirport.Id);
            flight.DepartureAt.Should().Be(model.DepartureAt);
            flight.ArrivalAirportId.Should().Be(arrivalAirport.Id);
            flight.ArrivalAt.Should().Be(model.ArrivalAt);
        }
    }
}