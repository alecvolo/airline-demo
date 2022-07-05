using System.Threading.Tasks;

namespace BuildingBlocks.EfCore.Outbox;

public interface IOutboxMessagePublisher
{
    Task PublishAsync(OutboxMessage message);
}