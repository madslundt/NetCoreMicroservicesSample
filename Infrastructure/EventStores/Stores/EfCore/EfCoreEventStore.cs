using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.EventStores.Stores.EfCore
{
    public class EfCoreEventStore : IStore
    {
        private readonly EfCoreEventStoreContext _context;

        public EfCoreEventStore(EfCoreEventStoreContext context)
        {
            _context = context;
        }

        public async Task Add(StreamState stream)
        {
            await _context.AddAsync(stream);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StreamState>> GetEvents(Guid aggregateId, int? version = null, DateTime? createdUtc = null)
        {
            var query = from stream in _context.Streams
                        where stream.AggregateId == aggregateId
                        select stream;

            if (version.HasValue)
            {
                query.Where(q => q.Version == version);
            }
            if (createdUtc.HasValue)
            {
                query.Where(q => q.CreatedUtc == createdUtc);
            }

            var result = await query.AsNoTracking().ToListAsync();

            return result;
        }

        public async Task<StreamState> GetStream(Guid streamId)
        {
            var query = from stream in _context.Streams
                        where stream.Id == streamId
                        select stream;

            var result = await query.AsNoTracking().FirstOrDefaultAsync();

            return result;
        }
    }
}
