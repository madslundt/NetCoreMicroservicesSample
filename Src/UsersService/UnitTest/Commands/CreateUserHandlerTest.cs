using AutoFixture.Xunit2;
using Events.Users;
using FluentAssertions;
using Infrastructure.Outbox;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UsersService.Commands;
using Xunit;

namespace UnitTest.Commands
{
    public class CreateUserHandlerTest : TestBase
    {

        [Theory]
        [InlineAutoData]
        public async Task Should_insert_User_in_db_when_Handler_is_called(CreateUserCommand.Command command, [Frozen] Mock<IOutboxListener> outboxMock)
        {
            var expectedUserCount = _db.Users.Count() + 1;
            var handler = new CreateUserCommand.Handler(outboxMock.Object, _db);

            await handler.Handle(command, CancellationToken.None);

            var actualUserCount = _db.Users.Count();
            actualUserCount.Should().Be(expectedUserCount);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_commit_UserCreated_to_Outbox_when_Handler_is_called(CreateUserCommand.Command command, [Frozen] Mock<IOutboxListener> outboxMock)
        {
            var handler = new CreateUserCommand.Handler(outboxMock.Object, _db);

            await handler.Handle(command, CancellationToken.None);

            outboxMock.Verify(x => x.Commit(It.IsAny<UserCreated>()), Times.Once);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_not_return_empty_guid_when_User_is_created(CreateUserCommand.Command command, [Frozen] Mock<IOutboxListener> outboxMock)
        {
            var handler = new CreateUserCommand.Handler(outboxMock.Object, _db);

            var result = await handler.Handle(command, CancellationToken.None);

            var actualId = result.Id;
            actualId.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_commit_same_Id_to_Outbox_when_Handler_is_called(CreateUserCommand.Command command, [Frozen] Mock<IOutboxListener> outboxMock)
        {
            var handler = new CreateUserCommand.Handler(outboxMock.Object, _db);

            var result = await handler.Handle(command, CancellationToken.None);

            var expectedId = result.Id;
            outboxMock.Verify(x => x.Commit(It.Is<UserCreated>(user => user.UserId == expectedId)));
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_save_User_with_requested_properties_to_db_when_saving_User(CreateUserCommand.Command command, [Frozen] Mock<IOutboxListener> outboxMock)
        {
            var handler = new CreateUserCommand.Handler(outboxMock.Object, _db);

            var result = await handler.Handle(command, CancellationToken.None);

            var actualUser = _db.Users.Select(u => new CreateUserCommand.Command
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).First();
            actualUser.Should().BeEquivalentTo(command);
        }
    }
}
