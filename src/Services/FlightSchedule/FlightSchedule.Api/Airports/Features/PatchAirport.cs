using AutoMapper;
using BuildingBlocks.EfCore;
using EntityFramework.Exceptions.Common;
using FlightSchedule.Api.Airports.Exceptions;
using FlightSchedule.Api.Airports.Models;
using FlightSchedule.Domain.EfCore;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Airports.Features;

public static class PatchAirport
{
    public record Command(Guid Id, JsonPatchDocument<AirportUpdateModel> JsonPath) : IRequest<AirportViewModel>, ITransactional;
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

            var airport = await _dbContext.Airports.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            if (airport == null)
            {
                throw new AirportNotFoundException();
            }
            AirportAdapter.ApplyTo(request.JsonPath.Operations, airport);
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