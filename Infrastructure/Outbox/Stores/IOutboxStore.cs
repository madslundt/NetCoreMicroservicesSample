using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Stores
{
    public interface IOutboxStore
    {
        Task Add(OutboxMessage message);
        Task<IEnumerable<Guid>> GetUnprocessedMessageIds();
        Task SetMessageToProcessed(Guid id);
        Task Delete(IEnumerable<Guid> ids);
        Task<OutboxMessage> GetMessage(Guid id);
    }
}
