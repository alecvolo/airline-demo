using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlightSchedule.Domain.EfCore;
    public class FlightDbContext : DbContext
    {
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public FlightDbContext(DbContextOptions<FlightDbContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FlightEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AirportConfigurationEntity());
            base.OnModelCreating(modelBuilder);
        }
    }

public class FlightDesignTimeDbContextFactory : IDesignTimeDbContextFactory<FlightDbContext>
{
    public FlightDbContext CreateDbContext(string[] args)
    {
        return new FlightDbContext(new DbContextOptionsBuilder<FlightDbContext>()
            .UseSqlServer("Server=127.0.0.1;Database=demo-test;User Id=sa;Password=Pass123!;").Options);
    }
}
