using DataModel.Models.User;
using System.Threading;
using System.Threading.Tasks;

namespace UsersService.Repository
{
    public interface IUserRepository
    {
        Task CreateUser(User user, CancellationToken cancellationToken);
    }
}
