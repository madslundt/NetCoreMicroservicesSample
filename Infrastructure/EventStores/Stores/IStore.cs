using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.EventStores.Stores
{
    public interface IStore
    {
        Task Add(StreamState stream);
        Task<IEnumerable<StreamState>> GetEvents(Guid aggregateId, int? version = null, DateTime? createdUtc = null);
    }
}
