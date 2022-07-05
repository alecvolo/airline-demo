using FlightSchedule.Domain;
using FlightSchedule.Domain.ValueObjects;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace FlightSchedule.Api.Airports.Features;

public class AirportAdapter : IObjectAdapter
{
    private readonly Dictionary<string, Action<Airport, Operation>> _mappedAction = new (StringComparer.CurrentCultureIgnoreCase)
        {
            { nameof(Airport.IataCode), (airport, operation) => airport.SetIataCode(new IataLocationCode((string)operation.value)) },
            {
                nameof(Airport.CityIataCode),
                (airport, operation) => airport.SetCityIataCode(new IataLocationCode((string)operation.value))
            },
            { nameof(Airport.Name), (airport, operation) => airport.SetName(new ObjectName((string)operation.value)) },
            { nameof(Airport.Address), (airport, operation) => airport.SetAddress((string)operation.value) },
        };
    private static string NormalizePropertyName(string path)
    {
        var pathPropertyName = path.Split("/", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();

        return pathPropertyName != null? string.Concat(char.ToUpper(pathPropertyName[0]), pathPropertyName.Substring(1)) : string.Empty;
    }

    public static void ApplyTo(IEnumerable<Operation> operations, Airport objectToApplyTo)
    {
        var adapter = new AirportAdapter();
        foreach (var operation in operations)
        {
            operation.Apply(objectToApplyTo, adapter);
        }
    }
    public void Update(Operation operation, object objectToApplyTo)
    {
        //if (_mappedEvents.TryGetValue(NormalizePropertyName(operation.path), out var eventCreator))
        //{
        //    ((Airport)objectToApplyTo).Apply(eventCreator.Invoke(operation));
        //}
        if (_mappedAction.TryGetValue(NormalizePropertyName(operation.path), out var action))
        {
            action((Airport)objectToApplyTo, operation);
        }
    }
    public void Add(Operation operation, object objectToApplyTo)
    {
        Update(operation, objectToApplyTo);
    }

    public void Copy(Operation operation, object objectToApplyTo)
    {
        throw new NotImplementedException();
    }

    public void Move(Operation operation, object objectToApplyTo)
    {
        throw new NotImplementedException();
    }

    public void Remove(Operation operation, object objectToApplyTo)
    {
        Update(operation, objectToApplyTo);
    }

    public void Replace(Operation operation, object objectToApplyTo)
    {
        Update(operation, objectToApplyTo);
    }
}