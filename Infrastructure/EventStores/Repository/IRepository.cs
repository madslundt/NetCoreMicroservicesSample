using Infrastructure.EventStores.Aggregates;
using System.Threading.Tasks;

namespace Infrastructure.EventStores.Repository
{
    public interface IRepository<TAggregate> where TAggregate : IAggregate
    {
        Task Add(TAggregate aggregate);
        Task Update(TAggregate aggregate);
        Task Delete(TAggregate aggregate);
    }
}
