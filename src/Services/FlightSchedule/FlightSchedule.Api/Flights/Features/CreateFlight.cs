using AutoMapper;
using BuildingBlocks.EfCore;
using EntityFramework.Exceptions.Common;
using FlightSchedule.Api.Flights.Exceptions;
using FlightSchedule.Api.Flights.Models;
using FlightSchedule.Domain;
using FlightSchedule.Domain.EfCore;
using FlightSchedule.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Flights.Features;

public static class CreateFlight
{
    public record Command(UpdateFlightModel Model): IRequest<FlightViewModel>, ITransactional;
    public class Handler : IRequestHandler<Command, FlightViewModel>
    {
        private readonly FlightDbContext _dbContext;

        public Handler(FlightDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<FlightViewModel> Handle(Command request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var airports = await _dbContext.Airports
                .Where(t => t.IataCode == (IataLocationCode)model.ArrivalAirport.ToUpper() || t.IataCode == (IataLocationCode)model.DepartureAirport.ToUpper())
                .Select(t => new {t.Id, IataCode = (string)t.IataCode }).ToArrayAsync(cancellationToken);
            var arrivalAirport = airports.FirstOrDefault(t => t.IataCode == model.ArrivalAirport.ToUpper());
            var departureAirport = airports.FirstOrDefault(t => t.IataCode == model.DepartureAirport.ToUpper());
            var validationFailures = new List<ValidationFailure>();
            if (arrivalAirport == null)
            {
                validationFailures.Add(new ValidationFailure(nameof(UpdateFlightModel.ArrivalAirport), $"{model.ArrivalAirport} is not found", model.ArrivalAirport));
            }
            if (departureAirport == null)
            {
                validationFailures.Add(new ValidationFailure(nameof(UpdateFlightModel.DepartureAirport), $"{model.DepartureAirport} is not found", model.DepartureAirport));
            }
            if (validationFailures.Any())
            {
                throw new ValidationException(validationFailures);
            }
            var flight = Flight.Create(new FlightNumber(model.FlightNumber), departureAirport!.Id, model.DepartureAt, arrivalAirport!.Id, model.ArrivalAt);
            await _dbContext.Flights.AddAsync(flight, cancellationToken);
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return new FlightViewModel()
                    { Id = flight.Id, FlightNumber = flight.FlightNumber,DepartureAirport = departureAirport.IataCode, DepartureAt = flight.DepartureAt, ArrivalAirport = arrivalAirport.IataCode, ArrivalAt = flight.ArrivalAt};
            }
            catch (UniqueConstraintException)
            {
                throw new DuplicateFlightNumberException(flight.FlightNumber);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}