using System.Diagnostics;
using BuildingBlocks.EfCore.Outbox;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace BuildingBlocks.EfCore.Tests.Outbox;

public class OutboxDbContextTests
{
    private readonly ITestOutputHelper _output;

    public OutboxDbContextTests(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact()]
    public void Should_Create_OutboxDbContext()
    {
        var builder = new DbContextOptionsBuilder<OutboxDbContext>();
        //builder.UseSqlServer(ConnectionDataString);
        //builder.UseInMemoryDatabase("test-db");
        builder.UseSqlite("DataSource=myshareddb3;mode=memory;cache=shared");
        builder.EnableDetailedErrors().LogTo(message =>
        {
            Debug.WriteLine(message);
            _output.WriteLine(message);
        });

        var dbContext = new OutboxDbContext(builder.Options);
        dbContext.Database.EnsureCreated();
    }
}