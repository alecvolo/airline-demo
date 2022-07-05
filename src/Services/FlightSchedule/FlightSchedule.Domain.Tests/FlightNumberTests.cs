using System;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Domain.Tests
{
    public class FlightNumberTests
    {
        [Theory]
        [InlineData("af1234", "AF", 1234)]
        [InlineData("af1", "AF", 1)]
        [InlineData("af001", "AF", 1)]
        public void Should_Create_FlightNumber_Via_String_Constructor(string value, string iataCode, ushort number)
        {
           var result = new FlightNumber(value);
           result.AirLineCode.Should().BeEquivalentTo(iataCode);
           result.Number.Should().Be(number);
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("af000")]
        [InlineData("af01z")]
        public void Should_Not_Create_FlightNumber_Via_String_Constructor(string value)
        {
            var action = () => new FlightNumber(value);
            action.Should().Throw<ArgumentException>();
        }
        [Theory]
        [InlineData("af", 1234, "AF1234")]
        [InlineData("af", 1, "AF1")]
        [InlineData("AF", 21, "AF21")]
        public void Should_Convert_To_String(string iataCode, ushort number, string stringValue)
        {
            var result = new FlightNumber(iataCode, number);
            ((string)result).Should().BeEquivalentTo(stringValue);
            result.Number.Should().Be(number);
        }
    }
}