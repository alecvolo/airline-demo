using FlightSchedule.Api.Flights.Helpers;
using FlightSchedule.Api.Flights.Models;
using FlightSchedule.Domain.EfCore;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Flights.Features;

public static class GetFlights
{
    public record Query(string? FlightNumber) : IRequest<IEnumerable<FlightViewModel>>;
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
        }
    }
    public class Handler : IRequestHandler<Query, IEnumerable<FlightViewModel>>
    {
        private readonly FlightDbContext _dbContext;

        public Handler(FlightDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<IEnumerable<FlightViewModel>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _dbContext.Flights.ProjectToFlightViewModel().ToListAsync(cancellationToken);
        }
    }
}