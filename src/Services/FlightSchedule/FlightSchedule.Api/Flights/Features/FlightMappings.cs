using AutoMapper;
using FlightSchedule.Api.Flights.Models;
using FlightSchedule.Domain;
using FlightSchedule.Domain.ValueObjects;

namespace FlightSchedule.Api.Flights.Features;

public class FlightMappings : Profile
{
    public FlightMappings()
    {
        CreateMap<FlightId, Guid>().ConstructUsing(v => (Guid)v);
        CreateMap<AirportId, Guid>().ConstructUsing(v => (Guid)v);
        CreateMap<IataLocationCode, string>().ConstructUsing(v => v);
        CreateMap<ObjectName, string>().ConstructUsing(v => v);
        CreateMap<Flight, UpdateFlightModel>()
            .Include<Flight, FlightViewModel>()
            .ForMember(p => p.FlightNumber, options => options.MapFrom(p => (string)p.FlightNumber))
            .ForMember(p=>p.DepartureAirport, options=>options.MapFrom(p=>(string)p.DepartureAirport.IataCode))
            .ForMember(p => p.ArrivalAirport, options => options.MapFrom(p => (string)p.ArrivalAirport.IataCode));
        CreateMap<Flight, FlightViewModel>();

    }
}