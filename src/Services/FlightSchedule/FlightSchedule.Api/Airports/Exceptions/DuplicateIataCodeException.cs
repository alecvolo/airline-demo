using BuildingBlocks.Web.Exceptions;

namespace FlightSchedule.Api.Airports.Exceptions;

public class DuplicateIataCodeException: DuplicateResourceException
{
    public DuplicateIataCodeException(string code): base($"Airport with \"{code}\" already exists")
    {
        
    }
}