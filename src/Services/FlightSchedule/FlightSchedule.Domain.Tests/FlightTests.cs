using System;
using System.Collections.Generic;
using BuildingBlocks.Helpers;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FlightSchedule.Domain.Tests
{
    public class FlightTests
    {
        public static List<object[]> ShouldCreateFlightTestData = new()
        {
            new object[] { "AF2345", (AirportId)Guid.NewGuid(), (AirportId)Guid.NewGuid(), DateTimeOffset.Now, DateTimeOffset.Now.AddHours(3) }
        };
        [Theory]
        [MemberData(nameof(ShouldCreateFlightTestData))]
        public void Should_Create_Flight(string flightNumber, AirportId departure, AirportId arrival, DateTimeOffset departureAt, DateTimeOffset arrivalAt)
        {
            var flight = Flight.Create(new FlightNumber(flightNumber), departure, departureAt, arrival, arrivalAt);
            ((string)flight.FlightNumber).Should().Be(flightNumber);
            flight.DepartureAt.Should().Be(departureAt.TrimToMinutes());
            flight.ArrivalAt.Should().Be(arrivalAt.TrimToMinutes());
        }
    }
}