using AutoMapper;
using FlightSchedule.Api.Airports.Features;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Api.Tests.Airports.Features
{
    public class AirportMappingsTests
    {

        [Fact()]
        public void Configuration_Should_Be_Valid()
        {
            var config = CreateMapperConfiguration();
        }

        public static MapperConfiguration CreateMapperConfiguration()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AirportMappings>());
            config.AssertConfigurationIsValid();
            return config;
        }

        [Fact()]
        public void Should_Map_Airport_To_AirportViewModel()
        {
            var mapper = CreateMapperConfiguration().CreateMapper();
            var airport = Airport.Create(new IataLocationCode("XXX"), new ObjectName("Some name"),
                "address");
            var model = mapper.Map<AirportViewModel>(airport);
            model.Id.Should().Be(airport.Id);
            model.IataCode.Should().BeEquivalentTo(airport.IataCode);
            model.Name.Should().BeEquivalentTo(airport.Name);
            model.Address.Should().BeEquivalentTo(airport.Address);
        }
        [Fact()]
        public void Should_Map_Airport_To_AirportUpdateModel()
        {
            var mapper = CreateMapperConfiguration().CreateMapper();
            var airport = Airport.Create(new IataLocationCode("XXX"), new ObjectName("Some name"),
                "address");
            var (iataCode, name, address) = mapper.Map<AirportUpdateModel>(airport);
            iataCode.Should().BeEquivalentTo(airport.IataCode);
            name.Should().BeEquivalentTo(airport.Name);
            address.Should().BeEquivalentTo(airport.Address);
        }
    }
}