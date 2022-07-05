using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EfCore.Outbox;

public class OutboxDbContext : DbContext
{
    public OutboxDbContext(DbContextOptions<OutboxDbContext> options) : base(options)
    {
    }

    protected OutboxDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.HasKey(p => p.EventId);
            builder.Property(p => p.EventTypeName).HasMaxLength(200);
            builder.HasIndex(p => new { p.State, p.EventDateTime });
        });
    }
}