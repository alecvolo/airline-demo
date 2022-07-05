using FlightSchedule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightSchedule.Domain.EfCore;

public class FlightEntityConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(t => t.Id)
            .HasConversion(flightId => (Guid)flightId, guid => FlightId.FromGuid(guid)).IsRequired();
        builder.Property(t => t.ArrivalAirportId)
            .HasConversion(airportId => (Guid)airportId, guid => (AirportId)guid);
        builder.Property(t => t.DepartureAirportId)
            .HasConversion(airportId => (Guid)airportId, guid => (AirportId)guid);
        builder.Property(p => p.DepartureAt).IsRequired();
        builder.Property(p => p.ArrivalAt).IsRequired();
        builder.Property(p => p.FlightNumber).HasConversion(flightNumber=>(string)flightNumber, str=>new FlightNumber(str)).IsRequired().HasMaxLength(20);
        builder.HasOne(p => p.ArrivalAirport).WithMany().HasForeignKey(p => p.ArrivalAirportId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p=>p.DepartureAirport).WithMany().HasForeignKey(p => p.DepartureAirportId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => p.FlightNumber).IsUnique();
    }
}
