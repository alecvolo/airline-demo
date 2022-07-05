using FlightSchedule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightSchedule.Domain.EfCore;
    public class AirportConfigurationEntity: IEntityTypeConfiguration<Airport>
    {
        public void Configure(EntityTypeBuilder<Airport> builder)
        {
            //builder.ToTable("Airports");
            builder.Property(p => p.Id).HasConversion(obj => (Guid)obj, dbValue => (AirportId)dbValue);
            builder.Property(p => p.IataCode).HasConversion(code=>(string)code, str=>(IataLocationCode)str).HasMaxLength(3).IsRequired();
            builder.Property(p => p.Name).HasConversion(name => (string)name, str => (ObjectName)str).IsRequired().HasMaxLength(80);
            builder.Property(p => p.CityIataCode).HasConversion(code => code == null ? null : (string)code, str => str == null ? null : (IataLocationCode)str).HasMaxLength(3);
            builder.Property(p => p.Address).HasMaxLength(256);

        builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.IataCode).IsUnique();
        }
    }

