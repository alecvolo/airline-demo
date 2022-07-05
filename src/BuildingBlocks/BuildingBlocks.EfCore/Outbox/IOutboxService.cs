using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.EfCore.Outbox;

public interface IOutboxService
{
    Task ProcessUnprocessed(CancellationToken cancellation = default);
}