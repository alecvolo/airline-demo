namespace FlightSchedule.Api.Airports.Models;

public record AirportUpdateModel
{
    public string IataCode { get; init; }
    public string Name { get; init; }
    public string? Address { get; init; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AirportUpdateModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
    public AirportUpdateModel(string iataCode, string name, string? address)
    {
        IataCode = iataCode;
        Name = name;
        Address = address;
    }

    public void Deconstruct(out string iataCode, out string name, out string? address)
    {
        iataCode = IataCode;
        name = Name;
        address = Address;
    }
}