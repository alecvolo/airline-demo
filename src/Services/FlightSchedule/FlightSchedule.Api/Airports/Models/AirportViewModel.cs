namespace FlightSchedule.Api.Airports.Models;

public record AirportViewModel: AirportUpdateModel
{
    public Guid Id { get; set; }
}