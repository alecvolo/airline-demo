using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain;

namespace FlightSchedule.Api.Airports.Helpers;

public static class AirportQueryableHelper
{
    public static IQueryable<AirportViewModel> ProjectToAirportViewModel(this IQueryable<Airport> queryable) =>
        queryable.Select(t => new AirportViewModel()
        {
            Id = t.Id, IataCode = t.IataCode, Name = t.Name, Address = t.Address
        });
}