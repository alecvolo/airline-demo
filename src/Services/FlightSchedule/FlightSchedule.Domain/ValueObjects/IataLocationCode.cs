using BuildingBlocks.Domain;

namespace FlightSchedule.Domain.ValueObjects;

public record IataLocationCode : Value<IataLocationCode>
{
    private readonly string _value;
    public IataLocationCode(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (!IsValid(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "IATA location code must contain 3 letters");
        }
        _value = value.ToUpper();
    }

    public override string ToString() => $"IataLocationCode {{ {_value} }}";

    //public static implicit operator string?(IataLocationCode? code) => code?._value;
    public static implicit operator string(IataLocationCode code) => code?._value;
    public static explicit operator IataLocationCode(string code) => new (code);

    //public static bool operator ==(IataLocationCode code, string stringValue)
    //{
    //    return code._value == stringValue;
    //}
    //public static bool operator !=(IataLocationCode code, string stringValue)
    //{
    //    return code._value != stringValue;
    //}

    //private static readonly Regex ValidCodeRegex = new ("^[A-Za-z]{3}$", RegexOptions.Compiled | RegexOptions.Singleline);
    public static bool IsValid(string? value)
    {
        return value== null || ( !string.IsNullOrWhiteSpace(value) && value.Length == 3 && char.IsLetter(value[0]) &&
               char.IsLetter(value[1]) && char.IsLetter(value[2])); // ValidCodeRegex.IsMatch(value);
    }

    public static IataLocationCode? CreateNullable(string? code) => code == null ? null : new IataLocationCode(code);

}
