using Events.Reviews;
using Infrastructure.Core.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataModel.UpdateHandlers
{
    public class ReviewDeletedHandler : IEventHandler<ReviewDeletedEvent>
    {
        private readonly DatabaseContext _db;

        public ReviewDeletedHandler(DatabaseContext db)
        {
            _db = db;
        }

        public async Task Handle(ReviewDeletedEvent @event, CancellationToken cancellationToken)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(m => m.Id == @event.ReviewId);

            if (review is null)
            {
                throw new ArgumentNullException($"Could not find {nameof(review)} with id '{@event.ReviewId}'");
            }

            _db.Remove(review);
            await _db.SaveChangesAsync();
        }
    }
}
