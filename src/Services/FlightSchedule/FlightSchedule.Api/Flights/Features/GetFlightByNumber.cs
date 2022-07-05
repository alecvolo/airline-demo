using FlightSchedule.Api.Flights.Helpers;
using FlightSchedule.Api.Flights.Models;
using FlightSchedule.Api.Validators;
using FlightSchedule.Domain.EfCore;
using FlightSchedule.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Flights.Features;

public static class GetFlightByNumber
{
    public record Query(string FlightNumber) : IRequest<FlightViewModel?>;
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(p => p.FlightNumber).FlightNumberMustBeValid();
        }
    }
    public class Handler : IRequestHandler<Query, FlightViewModel?>
    {
        private readonly FlightDbContext _dbContext;

        public Handler(FlightDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<FlightViewModel?> Handle(Query request, CancellationToken cancellationToken)
        {
            //https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations
            //https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities
            //https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-3/
            //https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-3/
            //https://github.com/fluxera/Fluxera.StronglyTypedId
            //var guid = Guid.NewGuid();
            return await _dbContext.Flights
                .Where(t=>t.FlightNumber == new FlightNumber(request.FlightNumber))
                //.Where(t => (Guid)t.Id == guid)
                //.Where(t => t.Id == guid)
                //.Where(t => t.Id == FlightId.FromGuid(guid))
                //.Where(t => EF.Functions.Like((string)t.FlightNumber, new (request.FlightNumber)))
                .ProjectToFlightViewModel()
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}