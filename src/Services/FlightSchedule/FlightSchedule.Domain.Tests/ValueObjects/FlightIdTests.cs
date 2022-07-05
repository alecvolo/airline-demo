using System;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Domain.Tests.ValueObjects;

public class FlightIdTests
{
    [Fact()]
    public void Should_Create_FlightId()
    {
        var id = new FlightId();
        ((Guid)id).Should().NotBeEmpty();
    }
    [Fact()]
    public void Should_Create_FlightId_WithGuid()
    {
        var guid = Guid.NewGuid();
        var id = new FlightId(guid);
        ((Guid)id).Should().Be(guid);
    }
    [Fact()]
    public void FlightIds_With_Same_Guid_Should_Be_Equal()
    {
        var guid = Guid.NewGuid();
        new FlightId(guid).Should().Be(new FlightId(guid));
        // ReSharper disable once EqualExpressionComparison
        (new FlightId(guid) == new FlightId(guid)).Should().BeTrue();
    }

}