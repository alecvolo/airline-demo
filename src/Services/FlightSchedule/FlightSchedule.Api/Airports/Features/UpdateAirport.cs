using AutoMapper;
using BuildingBlocks.EfCore;
using EntityFramework.Exceptions.Common;
using FlightSchedule.Api.Airports.Exceptions;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain.EfCore;
using FlightSchedule.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Airports.Features;

public static class UpdateAirport
{

    public record Command(Guid Id, string IataCode, string Name, string? Address) : IRequest<AirportViewModel>, ITransactional
    {
        public Command(Guid id, AirportUpdateModel model): this(id, model.IataCode, model.Name, model.Address){}
    }

    public class Handler : IRequestHandler<Command, AirportViewModel>
    {
        private readonly FlightDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(FlightDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        }
        public async Task<AirportViewModel> Handle(Command request, CancellationToken cancellationToken)
        {

            var airport = await _dbContext.Airports.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            if (airport == null)
            {
                throw new AirportNotFoundException();
            }
            airport.SetIataCode(new IataLocationCode(request.IataCode));
            airport.SetName(new ObjectName(request.Name));
            airport.SetAddress(request.Address);
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
    }
}