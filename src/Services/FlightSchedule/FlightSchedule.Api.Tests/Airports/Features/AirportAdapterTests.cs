using FlightSchedule.Api.Airports.Features;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Xunit;

namespace FlightSchedule.Api.Tests.Airports.Features
{
    public class AirportAdapterTests
    {
        [Theory()]
        [InlineData(" New Name ")]
        public void Should_Update_Name(string value)
        {
            var airport = Airport.Create(new IataLocationCode("XXX"), new ObjectName("Airport Name"));
            var operation = new Operation("replace", nameof(AirportUpdateModel.Name), null, value);
            var adapter = new AirportAdapter();
            adapter.Update(operation, airport);
            airport.Name.Should().Be(new ObjectName(value));
        }
        [Theory()]
        [InlineData("yyy")]
        public void Should_Update_IataCode(string value)
        {
            var airport = Airport.Create((IataLocationCode)"XXX", (ObjectName)"Airport Name");
            var operation = new Operation("replace", nameof(AirportUpdateModel.IataCode), null, value);
            var adapter = new AirportAdapter();
            adapter.Update(operation, airport);
            airport.IataCode.Should().Be(new IataLocationCode(value));
        }
        [Theory()]
        [InlineData("new address")]
        public void Should_Update_Address(string value)
        {
            var airport = Airport.Create((IataLocationCode)"XXX", (ObjectName)"Airport Name");
            var operation = new Operation("replace", nameof(AirportUpdateModel.Address), null, value);
            var adapter = new AirportAdapter();
            adapter.Update(operation, airport);
            airport.Address.Should().Be(value);
        }
    }
}