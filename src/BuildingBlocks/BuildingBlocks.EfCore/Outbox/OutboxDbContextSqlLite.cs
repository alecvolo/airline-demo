using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildingBlocks.EfCore.Outbox;

public class OutboxDbContextSqlLite : OutboxDbContext
{
    /// <summary>
    /// Options will be of this specific type and we pass this to the protected not typed base class constructor
    /// </summary>
    /// <param name="options"></param>
    public OutboxDbContextSqlLite(DbContextOptions<OutboxDbContextSqlLite> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OutboxMessage>(build =>
        {
            build.Property(p => p.EventDateTime).HasConversion(new DateTimeOffsetToBinaryConverter());
            //build.Property(p => p.UpdatedAt).HasConversion(new DateTimeOffsetToBinaryConverter());
        });
    }
}