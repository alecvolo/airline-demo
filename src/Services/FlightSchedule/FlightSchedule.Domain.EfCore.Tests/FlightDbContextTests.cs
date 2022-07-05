using System.Diagnostics;
using FlightSchedule.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlightSchedule.Domain.EfCore.Tests
{
    public class FlightDbContextTests
    {
        protected FlightDbContext GetFlightDbContext()
        {
            var builder = new DbContextOptionsBuilder<FlightDbContext>();
            //builder.UseSqlServer(ConnectionDataString);
            builder.UseInMemoryDatabase("test-db");
            builder.EnableDetailedErrors().LogTo(message => Debug.WriteLine(message));
            return new FlightDbContext(builder.Options);
        }

        [Fact()]
        public void FlightDbContextTest()
        {
            using var dbContext = GetFlightDbContext();
        }

        [Fact]
        public async Task Should_Insert_New_Airport()
        {
            await using var dbContext = GetFlightDbContext();
            var airPort = Airport.Create((IataLocationCode)"EWR", (ObjectName)"Newark Liberty International Airport");
            await dbContext.Airports.AddAsync(airPort);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Entries().Any(e => e.State != EntityState.Unchanged).Should().BeFalse();
        }
        [Fact]
        public async Task Should_Select_Airports()
        {
            await using var dbContext = GetFlightDbContext();
            if (!dbContext.Airports.Any())
            {
                await Should_Insert_New_Airport();
            }
            var airports = await dbContext.Airports.ToListAsync();
            airports.Should().NotBeEmpty();

            var result = await (from a in dbContext.Airports
                                select new { Id = (Guid)a.Id, a.Name, a.IataCode }).ToArrayAsync();
            result.Should().NotBeEmpty();
        }
        [Fact]
        public async Task Should_Select_Flight_And_Airports()
        {
            //await using var dbContext = GetFlightDbContext();
            //var result = await (from f in dbContext.Flights
            //                    join ds in dbContext.Airports on f.DepartureAirportId equals ds.Id into dss
            //                    from d in dss.DefaultIfEmpty()
            //                    join a in dbContext.Airports on f.ArrivalAirportId equals a.Id
            //                    select new { Id = (Guid)f.Id, f.FlightNumber, DepartureAirpotCode = (string)d.IataCode, ArrivalAirpotCode = (string)a.IataCode }).ToArrayAsync();
            //var fullData = await dbContext.Flights
            //    .Include(p => p.DepartureAirport).Include(p => p.ArrivalAirport)
            //    .Where(f =>  (f.DepartureAirport == null || f.DepartureAirport.IataCode == "JFK") && f.DepartureAt.Date >= DateTime.Today && f.DepartureAt.Date < DateTime.Today.AddDays(1))
            //    .AsNoTracking().ToListAsync()
            //    ;
            ////result.Should().NotBeEmpty();
        }

    }
}