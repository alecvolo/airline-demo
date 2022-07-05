using FlightSchedule.Api.Airports.Helpers;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain.EfCore;
using FlightSchedule.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Airports.Features;

public static class GetAirportById
{
    public record Query(Guid Id) : IRequest<AirportViewModel?>;
    public class Handler : IRequestHandler<Query, AirportViewModel?>
    {
        private readonly FlightDbContext _dbContext;

        public Handler(FlightDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<AirportViewModel?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _dbContext.Airports.Where(t => t.Id == new AirportId(request.Id)).ProjectToAirportViewModel()
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}