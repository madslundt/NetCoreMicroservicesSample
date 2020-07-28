using AutoFixture.Xunit2;
using DataModel.Models.User;
using FluentAssertions;
using Moq;
using System;
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
        public async Task Should_call_CreateUser_in_Repository_when_Handler_is_called(CreateUserCommand.Command command)
        {
            var handler = new CreateUserCommand.Handler(_repositoryMock.Object);
            await handler.Handle(command, CancellationToken.None);

            _repositoryMock.Verify(m => m.CreateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_call_CreateUser_with_FirstName_from_Command_when_Handler_is_called(CreateUserCommand.Command command)
        {
            var expectedFirstName = command.FirstName;

            var handler = new CreateUserCommand.Handler(_repositoryMock.Object);
            await handler.Handle(command, CancellationToken.None);

            _repositoryMock.Verify(m => m.CreateUser(It.Is<User>(x => x.FirstName == expectedFirstName), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_call_CreateUser_with_LastName_from_Command_when_Handler_is_called(CreateUserCommand.Command command)
        {
            var expectedLastName = command.LastName;

            var handler = new CreateUserCommand.Handler(_repositoryMock.Object);
            await handler.Handle(command, CancellationToken.None);

            _repositoryMock.Verify(m => m.CreateUser(It.Is<User>(x => x.LastName == expectedLastName), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_call_CreateUser_with_Email_from_Command_when_Handler_is_called(CreateUserCommand.Command command)
        {
            var expectedEmail = command.Email;

            var handler = new CreateUserCommand.Handler(_repositoryMock.Object);
            await handler.Handle(command, CancellationToken.None);

            _repositoryMock.Verify(m => m.CreateUser(It.Is<User>(x => x.Email == expectedEmail), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_call_CreateUser_with_Id_return_from_Handler(CreateUserCommand.Command command)
        {
            var handler = new CreateUserCommand.Handler(_repositoryMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);

            var expectedId = result.Id;
            _repositoryMock.Verify(m => m.CreateUser(It.Is<User>(x => x.Id == expectedId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineAutoData]
        public async Task Should_not_return_empty_guid_when_User_is_created(CreateUserCommand.Command command)
        {
            var handler = new CreateUserCommand.Handler(_repositoryMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);

            var actualId = result.Id;
            actualId.Should().NotBe(Guid.Empty);
        }
    }
}
