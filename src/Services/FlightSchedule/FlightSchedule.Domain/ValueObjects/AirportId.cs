using BuildingBlocks.Domain;

namespace FlightSchedule.Domain.ValueObjects;

public record AirportId : Value<AirportId>
{
    private readonly Guid _value;

    public AirportId()
    {
        _value = Guid.NewGuid();
    }

    public AirportId(Guid value)
    {
        _value = value;
    }
    public static implicit operator Guid(AirportId id) => id._value;
    public static explicit operator AirportId(Guid id) => new (id);

    public static AirportId FromGuid(Guid value) => new(value);
}