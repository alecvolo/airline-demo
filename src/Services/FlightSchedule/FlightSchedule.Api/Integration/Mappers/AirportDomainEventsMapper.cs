using BuildingBlocks.Domain.Event;
using FlightSchedule.Api.Integration.Events;
using FlightSchedule.Domain;

namespace FlightSchedule.Api.Integration.Mappers
{
    public class AirportDomainEventsMapper: IDomainEventMapper<Airport>
    {
        public IEnumerable<IIntegrationEvent?> Project(Airport aggregate, Airport originalAggregate,
            IDomainEvent<Airport> @event)
        {
            yield return @event switch
            {
                Airport.Events.IataCodeUpdated e =>  new AirportIataCodeChanged(aggregate.Id, e.Code,
                    originalAggregate.IataCode),
                _ => null
            };

        }
    }
}
