namespace FlightSchedule.Api.Flights.Models;

public record UpdateFlightModel
{
    public string FlightNumber { get; init; }
    public string DepartureAirport { get; init; }
    public string ArrivalAirport { get; init; }
    public DateTimeOffset DepartureAt { get; init; }
    public DateTimeOffset ArrivalAt { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public UpdateFlightModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}