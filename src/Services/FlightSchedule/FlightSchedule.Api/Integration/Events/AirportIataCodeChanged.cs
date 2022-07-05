using BuildingBlocks.Domain.Event;

namespace FlightSchedule.Api.Integration.Events;

public record AirportIataCodeChanged(Guid AirportId, string NewCode, string OldCode): AbstractEvent, IIntegrationEvent
{
}