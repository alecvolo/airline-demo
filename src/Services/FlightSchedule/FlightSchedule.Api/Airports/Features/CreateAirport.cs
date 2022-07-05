using AutoMapper;
using BuildingBlocks.EfCore;
using EntityFramework.Exceptions.Common;
using FlightSchedule.Api.Airports.Exceptions;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain;
using FlightSchedule.Domain.EfCore;
using FlightSchedule.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Airports.Features;


public static class CreateAirport
{
    public record Command(AirportUpdateModel Model) : IRequest<AirportViewModel>, ITransactional;

    public class Handler : IRequestHandler<Command, AirportViewModel>
    {
        private readonly FlightDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(FlightDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AirportViewModel> Handle(Command request, CancellationToken cancellationToken)
        {
            var airport = Airport.Create((IataLocationCode)request.Model.IataCode, (ObjectName)request.Model.Name, request.Model.Address);
            await _dbContext.Airports.AddAsync(airport, cancellationToken);
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return _mapper.Map<AirportViewModel>(airport);
            }
            catch (UniqueConstraintException)
            {
                throw new DuplicateIataCodeException(airport.IataCode);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Exception?> HandleDuplicateExceptionAsync(FlightDbContext context, Airport airport, CancellationToken cancellationToken = default)
        {
            try
            {
                context.ChangeTracker.Clear();
                context.Entry(airport).State = EntityState.Detached;

                if (await context.Airports.AnyAsync(t => t.IataCode == airport.IataCode,
                        cancellationToken: cancellationToken))
                {
                    return new DuplicateIataCodeException(airport.IataCode);
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}