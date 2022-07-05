using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Helpers.Tests;

public class DateTimeExtensionTests
{
    [Fact()]
    public void Should_Truncate_DateTime()
    {
        var value = DateTimeOffset.Now.FloorTime(TimeSpan.FromMinutes(30));
        value.Second.Should().Be(0);
        value.Millisecond.Should().Be(0);
        value.Minute.Should().BeOneOf(0, 30);

    }
}