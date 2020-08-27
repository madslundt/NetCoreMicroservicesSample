using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Stores
{
    public interface IStore
    {
        Task Add(OutboxMessage message);
        Task<IEnumerable<OutboxMessage>> GetUnprocessedMessages();
        Task SetMessageToProcessed(Guid id);
        Task Delete(IEnumerable<Guid> ids);

    }
}
