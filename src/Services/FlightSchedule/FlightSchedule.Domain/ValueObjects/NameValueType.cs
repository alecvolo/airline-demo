using BuildingBlocks.Domain;

namespace FlightSchedule.Domain.ValueObjects;

public record ObjectName : Value<ObjectName>
{
    private readonly string _value;
    public ObjectName(string value)
    {
        if (!IsValid(value))
        {
            throw new ArgumentNullException(nameof(value));
        }
        _value = value.Trim();
    }

    public override string ToString() => _value;
    public static implicit operator string(ObjectName name) => name._value;
    public static explicit operator ObjectName(string name) => new (name);

    public static bool IsValid(string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

}