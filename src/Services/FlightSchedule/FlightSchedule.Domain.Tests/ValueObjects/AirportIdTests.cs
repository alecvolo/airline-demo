using System;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Domain.Tests.ValueObjects;

public class AirportIdTests
{
    [Fact()]
    public void Should_Create_AirportId()
    {
        var id = new AirportId();
        ((Guid)id).Should().NotBeEmpty();
    }
    [Fact()]
    public void Should_Create_AirportId_WithGuid()
    {
        var guid = Guid.NewGuid();
        var id = new AirportId(guid);
        ((Guid)id).Should().Be(guid);
    }
    [Fact()]
    public void AirportIds_With_Same_Guid_Should_Be_Equal()
    {
        var guid = Guid.NewGuid();
        new AirportId(guid).Should().Be(new AirportId(guid));
    }
}