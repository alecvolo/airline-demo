using System.Data.Common;
using System.Diagnostics;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using BuildingBlocks.Domain.Event;
using BuildingBlocks.EfCore.Outbox;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BuildingBlocks.EfCore.Tests.Outbox
{
    public class EfOutboxServiceTests
    {

        public OutboxDbContext CreateOutboxDbContext(DbConnection? dbConnection = null)
        {
            var builder = new DbContextOptionsBuilder<OutboxDbContextSqlLite>()
                //.UseSqlServer(ConnectionDataString)
                //.UseInMemoryDatabase("test-db")
                .EnableDetailedErrors().LogTo(message => { Debug.WriteLine(message); });
            if (dbConnection == null)
            {
                builder.UseSqlite("DataSource=myshareddb;mode=memory;cache=shared");
            }
            else
            {
                builder.UseSqlite(dbConnection);

            }

            return new OutboxDbContextSqlLite(builder.Options);
        }

        [Theory]
        [AutoMoqData()]
        public async Task SaveAsyncTest([Frozen] Mock<IOutboxMessagePublisher> publisherMock, [Frozen] Mock<ILogger<EfOutboxService>> loggerMock, int eventsCount)
        {
            eventsCount %= 20;
            var efOutboxService = new EfOutboxService(CreateOutboxDbContext, publisherMock.Object, loggerMock.Object);
            var events =  Enumerable.Repeat(1, eventsCount).Select(t=>new TestEvent("propValue")).ToList();
            events[0].EventId.Should().Be(events[0].EventId);
            events[1].EventId.Should().NotBe(events[0].EventId);
            await using var connection = new SqliteConnection("DataSource=myshareddb;mode=memory;cache=shared");
            connection.Open();
            var traceId = Guid.NewGuid();
            await using (var dbContext = CreateOutboxDbContext(connection))
            {
                await dbContext.Database.EnsureCreatedAsync();
                var transaction = await dbContext.Database.BeginTransactionAsync();
                await efOutboxService.SaveAsync(dbContext.Database, events, traceId);
                await transaction.CommitAsync();
                dbContext.OutboxMessages.Count().Should().Be(events.Count);
            }

            await efOutboxService.PublishAsync(traceId);
            publisherMock.Verify(m=>m.PublishAsync(It.IsAny<OutboxMessage>()), Times.Exactly(events.Count));
        }
        [Theory]
        [AutoMoqData]
        public async Task ProcessUnprocessedTest([Frozen] Mock<IOutboxMessagePublisher> publisherMock, [Frozen] Mock<ILogger<EfOutboxService>> loggerMock)
        {
            var efOutboxService = new EfOutboxService(CreateOutboxDbContext, publisherMock.Object, loggerMock.Object);
            var events = Enumerable.Repeat(new TestEvent("propValue"), 5).ToList();
            await using var connection = new SqliteConnection("DataSource=myshareddb;mode=memory;cache=shared");
            connection.Open();
            var traceId = Guid.NewGuid();
            await using var dbContext = CreateOutboxDbContext(connection);
            await dbContext.Database.EnsureCreatedAsync();

            try
            {
                await efOutboxService.ProcessUnprocessed(null);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

    }

    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute(params object[] values) : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {

        }
    }

    public record TestEvent(string StringProperty) : AbstractEvent, IIntegrationEvent
    {

    }
}