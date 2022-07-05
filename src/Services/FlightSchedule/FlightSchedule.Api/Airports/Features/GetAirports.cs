using FlightSchedule.Api.Airports.Helpers;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain.EfCore;
using FlightSchedule.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Airports.Features;

public static class GetAirports
{
    public record Query(string? iataCode) : IRequest<IEnumerable<AirportViewModel>>;
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(p => p.iataCode).Must(value => value == null || IataLocationCode.IsValid(value)).WithMessage("IATA code must contain 3 letters");
        }
    }
    public class Handler : IRequestHandler<Query, IEnumerable<AirportViewModel>>
    {
        private readonly FlightDbContext _dbContext;

        public Handler(FlightDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<IEnumerable<AirportViewModel>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Airports.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.iataCode))
            {
                query = query.Where(t => t.IataCode == new IataLocationCode(request.iataCode));
            }
            return await query.ProjectToAirportViewModel().ToListAsync(cancellationToken);
        }
    }
}