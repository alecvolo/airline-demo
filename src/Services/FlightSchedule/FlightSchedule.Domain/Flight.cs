using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Event;
using BuildingBlocks.Helpers;
using FlightSchedule.Domain.ValueObjects;

namespace FlightSchedule.Domain;

public class Flight : AggregateRoot<Flight, FlightId>
{
    protected Flight()
    {
    }
    public FlightNumber FlightNumber { get; private set; }

    public DateTimeOffset DepartureAt { get; private set; }
    public AirportId DepartureAirportId { get; private set; }
    public AirportId ArrivalAirportId { get; private set; }
    public DateTimeOffset ArrivalAt { get; private set; }

    public Airport DepartureAirport { get; private set; }
    public Airport ArrivalAirport { get; private set; }

    public static Flight Create(FlightNumber flightNumber, AirportId departureAirportId, DateTimeOffset departureAt,
        AirportId arrivalAirportId, DateTimeOffset arrivalAt)
    {
        departureAt = departureAt.TrimToMinutes();
        arrivalAt = arrivalAt.TrimToMinutes();
        if (departureAt > arrivalAt)
        {
            throw new ArgumentOutOfRangeException(nameof(arrivalAt),
                "Arrival date must be greater than departure date");
        }
        if (arrivalAt > departureAt.AddDays(1))
        {
            throw new ArgumentOutOfRangeException(nameof(arrivalAt),
                "Arrival date must be not a day greater than departure date");
        }

        var result = new Flight();
        result.Apply(new Events.FlightCreated(new FlightId(), flightNumber, departureAirportId, departureAt,  arrivalAirportId,  arrivalAt));
        
        return new Flight
        {
            Id = new FlightId(Guid.NewGuid()),
            FlightNumber = flightNumber,
            DepartureAirportId = departureAirportId,
            DepartureAt = departureAt.TrimToMinutes(),
            ArrivalAirportId = arrivalAirportId,
            ArrivalAt = arrivalAt.TrimToMinutes(),
        };
    }
    public static Flight Create(FlightNumber flightNumber, Airport departureAirport, DateTimeOffset departureAt,
        Airport arrivalAirport, DateTimeOffset arrivalAt)
    {
        var result = Flight.Create(flightNumber, departureAirport.Id, departureAt, arrivalAirport.Id, arrivalAt);
        result.ArrivalAirport = arrivalAirport;
        result.DepartureAirport = departureAirport;
        return result;
    }

    protected override void When(IDomainEvent<Flight> @event)
    {
      var _ =  @event switch
        {
            Events.FlightCreated e => With(() =>
            {
                Id = e.Id;
                FlightNumber = e.FlightNumber;
                DepartureAirportId = e.DepartureAirportId;
                DepartureAt = e.DepartureAt;
                ArrivalAirportId = e.ArrivalAirportId;
                ArrivalAt = e.ArrivalAt;
            }),
            
           _ => With()
        };
    }

    protected override void EnsureValidState()
    {
    }

    public static class Events
    {
        public record FlightCreated(FlightId Id, FlightNumber FlightNumber, AirportId DepartureAirportId, DateTimeOffset DepartureAt,
            AirportId ArrivalAirportId, DateTimeOffset ArrivalAt) : AbstractEvent, IDomainEvent<Flight> {}
    }
}

