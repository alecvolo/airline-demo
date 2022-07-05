using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Domain.Tests.ValueObjects;

public class FlightNumberTests
{
    [Fact()]
    public void Should_Create_FlightNumber()
    {
        var flightNumber = new FlightNumber("al123");
    }
    [Fact()]
    public void Should_Create_FlightNumber_WithCodeAndNumber()
    {
        var flightNumber = new FlightNumber("xx", 123);
    }
    [Fact()]
    public void FlightNumber_With_Same_Arguments_Should_Be_Equal()
    {
        var flightNumber = "aa123";
        new FlightNumber(flightNumber).Should().Be(new FlightNumber(flightNumber));
        // ReSharper disable once EqualExpressionComparison
        (new FlightNumber(flightNumber) == new FlightNumber(flightNumber)).Should().BeTrue();
    }

}