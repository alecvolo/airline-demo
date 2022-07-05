using BuildingBlocks.Web.Exceptions;

namespace FlightSchedule.Api.Airports.Exceptions
{
    public class AirportNotFoundException: ResourceNotFoundException
    {
        public AirportNotFoundException()
        {
        }

        public AirportNotFoundException(string? message) : base(message)
        {
        }
    }
}
