using System;
using FlightSchedule.Domain.ValueObjects;
using Xunit;

namespace FlightSchedule.Domain.Tests.ValueObjects
{
    public class IataLocationCodeTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("q")]
        [InlineData("aa ")]
        [InlineData("aaaa")]
        [InlineData("12a")]
        [InlineData("_$%")]
        public void Should_Not_Be_Able_To_Create_From_String(string value)
        {
            Assert.ThrowsAny<ArgumentException>(() => (IataLocationCode)value);
        }
        [Theory]
        [InlineData("NYC")]
        [InlineData("nyc")]
        public void Should_Be_Able_To_Create_From_String(string value)
        {
            var code = (IataLocationCode)value;
        }
    }
}