using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightSchedule.Domain;
using FlightSchedule.Domain.EfCore;
using FlightSchedule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FlightSchedule.Api.Tests.Fixtures;

public static class AirportDaoExtensions
{
    public static async Task<Airport> CreateAirportAsync(this FlightDbContext dbContext, string iataCode, string name,
        string? address = null)
    {
        var airport = Airport.Create((IataLocationCode)iataCode, (ObjectName)name, address);
        await dbContext.Airports.AddAsync(airport);
        await dbContext.SaveChangesAsync();
        return airport;

    }

    public static async Task<Airport?> GetAirportByIdAsync(this FlightDbContext dbContext, Guid id) =>
        await dbContext.Airports.FirstOrDefaultAsync(t=>t.Id == id);
    public static async Task<List<Airport>> ReadAllAirports(this FlightDbContext dbContext) =>
        await dbContext.Airports.ToListAsync();

    public static async Task DeleteAirportAsync(this FlightDbContext dbContext, Guid id)
    {
        var airport =await dbContext.GetAirportByIdAsync(id);
        if (airport != null)
        {
            dbContext.Airports.Remove(airport);
            await dbContext.SaveChangesAsync();
        }

    }

}