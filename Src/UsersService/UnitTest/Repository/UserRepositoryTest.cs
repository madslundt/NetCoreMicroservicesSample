using AutoFixture.Xunit2;
using DataModel.Models.User;
using Events.Users;
using FluentAssertions;
using Infrastructure.Core;
using Moq;
using System;
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
            var repository = new UserRepository(_outboxMock.Object, _db, new TransactionId());
            await repository.CreateUser(expectedUser);

            var actualUser = _db.Users.First();
            actualUser.Should().BeEquivalentTo(expectedUser);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_add_UserCreatedEvent_to_Outbox_when_adding_User(User user)
        {
            var repository = new UserRepository(_outboxMock.Object, _db, new TransactionId());
            await repository.CreateUser(user);

            _outboxMock.Verify(m => m.Add(It.IsAny<UserCreatedEvent>(), It.IsAny<Guid>()), Times.Once);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_not_commit_UserCreatedEvent_to_Outbox(User user)
        {
            var repository = new UserRepository(_outboxMock.Object, _db, new TransactionId());
            await repository.CreateUser(user);

            _outboxMock.Verify(m => m.Commit(It.IsAny<UserCreatedEvent>()), Times.Never);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_use_TransactionId_when_adding_UserCreatedEvent_to_Outbox(User user)
        {
            var transactionIdMock = new Mock<TransactionId>();
            transactionIdMock.SetupGet(m => m.Value).Returns(Guid.NewGuid());


            var repository = new UserRepository(_outboxMock.Object, _db, transactionIdMock.Object);
            await repository.CreateUser(user);

            transactionIdMock.VerifyGet(m => m.Value, Times.Once);
        }
    }
}
