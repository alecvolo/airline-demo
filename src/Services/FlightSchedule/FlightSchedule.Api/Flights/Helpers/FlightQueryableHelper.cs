using FlightSchedule.Api.Flights.Models;
using FlightSchedule.Domain;

namespace FlightSchedule.Api.Flights.Helpers;

public static class FlightQueryableHelper
{
    public static IQueryable<FlightViewModel> ProjectToFlightViewModel(this IQueryable<Flight> queryable) =>
        queryable.Select(t => new FlightViewModel()
        {
            Id = t.Id, FlightNumber = t.FlightNumber, DepartureAirport = t.DepartureAirport.IataCode,
            DepartureAt = t.DepartureAt, ArrivalAirport = t.ArrivalAirport.IataCode, ArrivalAt = t.ArrivalAt,
        });
}