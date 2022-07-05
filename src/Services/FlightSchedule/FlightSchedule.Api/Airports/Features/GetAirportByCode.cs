using FlightSchedule.Api.Airports.Helpers;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Api.Validators;
using FlightSchedule.Domain.EfCore;
using FlightSchedule.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Airports.Features;

public static class GetAirportByCode
{
    public record Query(string? IataCode) : IRequest<AirportViewModel?>;
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(p => p.IataCode).IataLocationCodeMustBeValid().WithMessage("IATA code must contain 3 letters");
        }
    }
    public class Handler : IRequestHandler<Query, AirportViewModel?>
    {
        private readonly FlightDbContext _dbContext;

        public Handler(FlightDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<AirportViewModel?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _dbContext.Airports.Where(t => t.IataCode == new IataLocationCode(request.IataCode))
                .ProjectToAirportViewModel().FirstOrDefaultAsync(cancellationToken);
        }
    }
}