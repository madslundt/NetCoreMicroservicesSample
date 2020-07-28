using AutoFixture.Xunit2;
using DataModel.Models.User;
using Events.Users;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using UsersService.Repository;
using Xunit;

namespace UnitTest.Repository
{
    public class UserRepositoryTest : TestBase
    {
        [Theory]
        [InlineAutoData]
        public async Task Should_add_User_to_db(User expectedUser)
        {
            var repository = new UserRepository(_outboxMock.Object, _db);
            await repository.CreateUser(expectedUser);

            var actualUser = _db.Users.First();
            actualUser.Should().BeEquivalentTo(expectedUser);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_commit_UserCreatedEvent_to_Outbox_when_adding_User(User user)
        {
            var repository = new UserRepository(_outboxMock.Object, _db);
            await repository.CreateUser(user);

            _outboxMock.Verify(m => m.Commit(It.IsAny<UserCreatedEvent>()), Times.Once);
        }
    }
}
