using System.Text.RegularExpressions;

namespace FlightSchedule.Domain.ValueObjects;

public struct AStruct
{
    public Guid Id { get; set; }
    public static AStruct Empty { get; }
}
public interface ITypedId{}
//https://stackoverflow.com/questions/63671/is-it-safe-for-structs-to-implement-interfaces
//The fact that a struct can implement an interface is well known and so is the fact that casting a value type into an interface leads to boxing of the value type.
//This is because methods in interfaces are defined as virtual and to resolve virtual references, vtable (method table) look up is required.
//Since value types do not have pointers to vtable they are first boxed into a reference type and then the look up happens.
public readonly struct FlightNumberStruct: ITypedId
{
    public const string FlightNumberInvalidMessage = "Flight number should contain 2 letter IATA flight codes and a number from 1 to 9999";
    public string AirLineCode { get; }
    public ushort Number { get; }

    public FlightNumberStruct(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!IsValid(value, out var match))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, FlightNumberInvalidMessage);
        }
        AirLineCode = match!.Groups[1].Value.ToUpper();
        Number = ushort.Parse(match.Groups[2].Value);
    }
    public FlightNumberStruct(string airLineCode, ushort flightNumber)
    {
        if (flightNumber is 0 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(flightNumber), flightNumber, "Number should be between 1 and 9999");
        }
        if (airLineCode is not { Length: 2 } || !(char.IsLetterOrDigit(airLineCode, 0) && char.IsLetterOrDigit(airLineCode, 1)))
        {
            throw new ArgumentOutOfRangeException(nameof(flightNumber), flightNumber, "Airline IATA code must contain 2 letters or digits");
        }
        AirLineCode = airLineCode.ToUpper();
        Number = flightNumber;
    }

    public override string ToString() => $"Flight number {{ {(string)this} }}";
    public override bool Equals(object? obj) => obj is FlightNumberStruct @struct && Equals(@struct);

    public bool Equals(FlightNumberStruct other) => AirLineCode == other.AirLineCode && Number == other.Number;
    public override int GetHashCode() => HashCode.Combine(AirLineCode, Number);

    public static implicit operator string(FlightNumberStruct flightNumber) => $"{flightNumber.AirLineCode}{flightNumber.Number}";
    public static explicit operator FlightNumberStruct(string code) => new (code);
    public static bool operator  == (FlightNumberStruct left, FlightNumberStruct right) => left.Equals(right);
    public static bool operator !=(FlightNumberStruct left, FlightNumberStruct right) => !left.Equals(right);


    private static void test()
    {
        AStruct? ast = null;
        var b = ast.GetValueOrDefault();
        b.Id = Guid.Empty;
        //FlightNumberStruct variable ;
        //var string1 = variable.AirLineCode;
    }
    private static readonly Regex ValidCodeRegex = new ("^([A-Za-z0-9]{2})(\\d{1,4})$", RegexOptions.Compiled | RegexOptions.Singleline);
    public static bool IsValid(string value)
    {
        return IsValid(value, out var _);

        //try
        //{
        //    new FlightNumber(value);
        //    return true;
        //}
        //catch
        //{
        //    return false;
        //}
    }
    private static bool IsValid(string value, out Match? match)
    {
        if (value == null)
        {
            match = null;
            return false;
        }
        match = ValidCodeRegex.Match(value);
        return match.Success && match.Groups[2].Value.Any(ch => ch != '0');
    }

    public static FlightNumberStruct? Create(string? code) => code == null ? null : new FlightNumberStruct(code);

}