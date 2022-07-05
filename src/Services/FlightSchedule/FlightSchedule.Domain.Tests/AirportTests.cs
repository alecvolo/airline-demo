using System;
using System.Linq;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Domain.Tests
{
    public class AirportTests
    {
        private Airport CreateTestAirport() => Airport.Create(new IataLocationCode("XXX"), new ObjectName("name"),
            "address", new IataLocationCode("yyy"));
        [Fact()]
        public void Should_Create_Airport()
        {
            var iataCode = new IataLocationCode("xxx");
            var name = new ObjectName("name");
            var cityIataCode = new IataLocationCode("xxx");
            var address = "some address";
            var airport = Airport.Create(iataCode, name, address, cityIataCode);
            airport.Id.Should().NotBeNull();
            ((Guid)airport.Id).Should().NotBeEmpty();
            airport.IataCode.Should().Be(iataCode);
            airport.Name.Should().Be(name);
            airport.Address.Should().Be(address);
            airport.CityIataCode.Should().Be(cityIataCode);
            airport.GetDomainEvents().LastOrDefault().Should().BeOfType<Airport.Events.Created>();
        }
        [Fact()]
        public void Should_Update_IataCode()
        {
            var airport = CreateTestAirport();
            var iataCode = new IataLocationCode("aaa");
            iataCode.Should().NotBe(airport.IataCode);
            airport.SetIataCode(iataCode);
            airport.IataCode.Should().Be(iataCode);
            airport.GetDomainEvents().LastOrDefault().Should().BeOfType<Airport.Events.IataCodeUpdated>();
        }
        [Fact()]
        public void Should_Update_CityIataCode()
        {
            var airport = CreateTestAirport();
            var iataCode = new IataLocationCode("aaa");
            iataCode.Should().NotBe(airport.CityIataCode);
            airport.SetCityIataCode(iataCode);
            airport.CityIataCode.Should().Be(iataCode);
            airport.GetDomainEvents().LastOrDefault().Should().BeOfType<Airport.Events.CityIataCodeUpdated>();
        }
        [Fact()]
        public void Should_Update_Name()
        {
            var airport = CreateTestAirport();
            var name = new ObjectName("new name");
            name.Should().NotBe(airport.Name);
            airport.SetName(name);
            airport.Name.Should().Be(name);
            airport.GetDomainEvents().LastOrDefault().Should().BeOfType<Airport.Events.NameUpdated>();
        }
        [Fact()]
        public void Should_Update_Address()
        {
            var airport = CreateTestAirport();
            var address = "new address";
            address.Should().NotBe(airport.Address);
            airport.SetAddress(address);
            airport.Address.Should().Be(address);
            airport.GetDomainEvents().LastOrDefault().Should().BeOfType<Airport.Events.AddressUpdated>();
        }
    }
}