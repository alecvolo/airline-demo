using BuildingBlocks.Web.Exceptions;

namespace FlightSchedule.Api.Flights.Exceptions;

public class DuplicateFlightNumberException : DuplicateResourceException
{
    public DuplicateFlightNumberException(string number) : base($"Flight with \"{number}\" already exists")
    {

    }
}