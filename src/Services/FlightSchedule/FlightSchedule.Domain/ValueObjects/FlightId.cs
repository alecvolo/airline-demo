using BuildingBlocks.Domain;

namespace FlightSchedule.Domain.ValueObjects;

public record FlightId : Value<FlightId>
{
    private readonly Guid _value;
    public FlightId()
    {
        _value = Guid.NewGuid();
    }
    public FlightId(Guid value)
    {
        _value = value;
    }

    public static implicit operator Guid(FlightId id) => id._value;
    public static explicit operator FlightId(Guid id) => new FlightId(id);

    public static FlightId FromGuid(Guid value) => new(value);
}