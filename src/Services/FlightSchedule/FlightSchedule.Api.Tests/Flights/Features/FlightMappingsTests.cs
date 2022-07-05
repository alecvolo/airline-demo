using System;
using AutoMapper;
using FlightSchedule.Api.Flights.Features;
using FlightSchedule.Api.Flights.Models;
using FlightSchedule.Domain;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Api.Tests.Flights.Features
{
    public class FlightMappingsTests
    {

        [Fact()]
        public void Configuration_Should_Be_Valid()
        {
            var config = CreateMapperConfiguration();
        }

        public static MapperConfiguration CreateMapperConfiguration()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<FlightMappings>());
            config.AssertConfigurationIsValid();
            return config;
        }

        [Fact()]
        public void Should_Map_Flight_To_Flight_ViewModel()
        {
            var mapper = CreateMapperConfiguration().CreateMapper();
            var departureAirport = Airport.Create(new IataLocationCode("XXX"), new ObjectName("Some name"),
                "address");
            var arrivalAirport = Airport.Create(new IataLocationCode("YYY"), new ObjectName("Some name"),
                "address");
            var flight = Flight.Create(new FlightNumber("AF", 123), departureAirport, DateTimeOffset.Now,
                arrivalAirport, DateTimeOffset.Now.AddHours(3));
            var model = mapper.Map<FlightViewModel>(flight);
            model.Id.Should().Be(flight.Id);
            model.FlightNumber.Should().BeEquivalentTo(flight.FlightNumber);
            model.DepartureAirport.Should().BeEquivalentTo(flight.DepartureAirport.IataCode);
            model.DepartureAt.Should().Be(flight.DepartureAt);
            model.ArrivalAirport.Should().BeEquivalentTo(flight.ArrivalAirport.IataCode);
            model.ArrivalAt.Should().Be(flight.ArrivalAt);
        }
    }
}