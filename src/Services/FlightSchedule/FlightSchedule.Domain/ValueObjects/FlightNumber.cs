using System.Text.RegularExpressions;
using BuildingBlocks.Domain;

namespace FlightSchedule.Domain.ValueObjects;

public record FlightNumber : Value<FlightNumber>
{
    public const string FlightNumberInvalidMessage = "Flight number should contain 2 letter IATA flight codes and a number from 1 to 9999";
    public string AirLineCode { get; }
    public ushort Number { get; }

    public FlightNumber(string value)
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
    public FlightNumber(string airLineCode, ushort flightNumber)
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

    public static implicit operator string(FlightNumber flightNumber) => $"{flightNumber.AirLineCode}{flightNumber.Number}";
    public static explicit operator FlightNumber(string code) => new (code);


    private static readonly Regex ValidCodeRegex = new ("^([A-Za-z0-9]{2})(\\d{1,4})$", RegexOptions.Compiled | RegexOptions.Singleline);
    public static bool IsValid(string? value)
    {
        return IsValid(value, out var _);
    }
    private static bool IsValid(string? value, out Match? match)
    {
        if (value == null)
        {
            match = null;
            return true;
        }
        match = ValidCodeRegex.Match(value);
        return match.Success && match.Groups[2].Value.Any(ch => ch != '0');
   }

    public static FlightNumber? Create(string? code) => code == null ? null : new FlightNumber(code);

}
