namespace FlightSchedule.Api.Flights.Models;

public record FlightViewModel: UpdateFlightModel
{
    public Guid Id { get; set; }
}