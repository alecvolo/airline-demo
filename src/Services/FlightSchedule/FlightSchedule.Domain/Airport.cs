using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Event;
using FlightSchedule.Domain.ValueObjects;

namespace FlightSchedule.Domain;

public abstract record StatedMarker;
public class StateRecord<T> where T: StatedMarker
{
    private T instance;
}

public class A : StateRecord<A.V>
{
    public record V: StatedMarker
    {
        
    }
}

public class Airport : AggregateRoot<Airport, AirportId>
{
    public record State
    {
        public IataLocationCode IataCode { get; set; }
        public ObjectName Name { get; set; }
        public IataLocationCode? IataCityCode { get; set; }
        public string? Address { get; set; }
    }

    private State _state = new();

    protected Airport() : base()
    {
        Id = AirportId.FromGuid(Guid.NewGuid());
    }

    public IataLocationCode IataCode
    {
        get => _state.IataCode;
        private set => _state.IataCode = value;
    }

    public ObjectName Name
    {
        get => _state.Name;
        private set => _state.Name = value;
    }

    public IataLocationCode? CityIataCode
    {
        get => _state.IataCityCode;
        private set => _state.IataCityCode = value;
    }

    public string? Address
    {
        get => _state.Address;
        private set => _state.Address = value;
    }

    protected override void When(IDomainEvent<Airport> @event)
    {

        var _ = @event switch
        {
            Events.Created ev => With(() =>
            {
                Id = ev.Id;
                IataCode = ev.IataCode;
                CityIataCode = ev.CityIataCode;
                Name = ev.Name;
                Address = ev.Address;

            }),
            Events.IataCodeUpdated ev => With(() => IataCode = ev.Code),
            Events.AddressUpdated ev => With(() => Address = ev.Address),
            Events.NameUpdated ev => With(() => Name = ev.Name),
            Events.CityIataCodeUpdated ev => With(() => CityIataCode = ev.Code),
            _ => With()
        };
    }

    protected override void EnsureValidState()
    {
    }

    protected State When2(object @event)
    {

        var newState = _state with { };

        newState = @event switch
        {
            Events.Created ev => With(newState, state =>
            {
                state.IataCode = ev.IataCode;
                state.IataCityCode = ev.CityIataCode;
                state.Name = ev.Name;
                state.Address = ev.Address;
            }),
            Events.IataCodeUpdated ev => With(newState, state => state.IataCode = new IataLocationCode(ev.Code)),
            Events.AddressUpdated ev => With(newState, state => state.Address = ev.Address),
            Events.CityIataCodeUpdated ev => With(newState,
                state => state.IataCityCode = ev.Code),
            _ => newState
        };
        if (_state != newState!)
        {
            _state = newState;
            //version ++
        }

        return _state;
    }


    protected State With(State state, Action<State> action)
    {
        action(state);
        return state;
    }


    public static Airport Create(IataLocationCode iataCode, ObjectName airportName, string? address = null,
        IataLocationCode? cityIataCode = null)
    {
        var result = new Airport();
        result.Apply(new Events.Created(new AirportId(), iataCode, airportName, address, cityIataCode));
        return result;
    }

    public void SetIataCode(IataLocationCode code)
    {
        if (IataCode != code)
        {
            Apply(new Events.IataCodeUpdated(code));
        }

    }

    public void SetCityIataCode(IataLocationCode? code)
    {
        if (CityIataCode != code)
        {
            Apply(new Events.CityIataCodeUpdated(code));
        }
    }
    public void SetName(ObjectName name)
    {
        if (Name != name)
        {
            Apply(new Events.NameUpdated(name));
        }
    }

    public void SetAddress(string? address)
    {
        if (Address != address)
        {
            Apply(new Events.AddressUpdated(address));
        }
    }

    public static class Events
    {
        public record AddressUpdated(string? Address) : AbstractEvent, IDomainEvent<Airport>;

        public record NameUpdated(ObjectName Name) : AbstractEvent, IDomainEvent<Airport>;

        public record IataCodeUpdated(IataLocationCode Code) : AbstractEvent, IDomainEvent<Airport>;

        public record CityIataCodeUpdated(IataLocationCode? Code) : AbstractEvent, IDomainEvent<Airport>;

        public record Created(AirportId Id, IataLocationCode IataCode, ObjectName Name, string? Address = null, IataLocationCode? CityIataCode = null): AbstractEvent, IDomainEvent<Airport>;
    }
}


