using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Outbox.Stores.EfCore
{
    public class EfCoreOutboxStore : IOutboxStore
    {
        private readonly EfCoreOutboxContext _context;

        public EfCoreOutboxStore(EfCoreOutboxContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }

        public async Task Add(OutboxMessage message)
        {
            await _context.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(IEnumerable<Guid> ids)
        {
            var messages = ids.Select(id => new OutboxMessage(id));
            _context.RemoveRange(messages);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessages()
        {
            var query = from message in _context.OutboxMessages
                        where !message.Processed.HasValue
                        select message;

            var result = await query.ToListAsync();

            return result;
        }

        public async Task SetMessageToProcessed(Guid id)
        {
            var message = new OutboxMessage(id);
            message.Processed = DateTime.UtcNow;

            _context.OutboxMessages.Attach(message);
            _context.Entry(message).Property(p => p.Processed).IsModified = true;

            await _context.SaveChangesAsync();
        }
    }
}
