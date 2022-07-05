using System.Diagnostics;
using BuildingBlocks.EfCore.Outbox;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace BuildingBlocks.EfCore.Tests.Outbox;

public class OutboxDbContextSqlLiteTests
{
    private readonly ITestOutputHelper _output;

    public OutboxDbContextSqlLiteTests(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact()]
    public void Should_Create_OutboxDbContext()
    {
        var builder = new DbContextOptionsBuilder<OutboxDbContextSqlLite>();
        //builder.UseSqlServer(ConnectionDataString);
        //builder.UseInMemoryDatabase("test-db");
        builder.UseSqlite("DataSource=myshareddb2;mode=memory;cache=shared");
        builder.EnableDetailedErrors().LogTo(message =>
        {
            Debug.WriteLine(message);
            _output.WriteLine(message);
        });

        var dbContext = new OutboxDbContextSqlLite(builder.Options);
        dbContext.Database.EnsureCreated();
    }
}