using AutoMapper;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain;
using FlightSchedule.Domain.ValueObjects;

namespace FlightSchedule.Api.Airports.Features;

public class AirportMappings : Profile
{
    public AirportMappings()
    {
        CreateMap<AirportId, Guid>().ConstructUsing(v => (Guid)v);
        CreateMap<IataLocationCode, string>().ConstructUsing(v => v);
        CreateMap<ObjectName, string>().ConstructUsing(v => v);
        CreateMap<Airport, AirportViewModel>();
        CreateMap<Airport, AirportUpdateModel>();
    }
}