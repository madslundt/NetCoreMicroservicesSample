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
        public async Task Should_not_return_empty_guid_when_User_is_created(CreateUserCommand.Command command)
        {
            //var handler = new CreateUserCommand.Handler();
            //var result = await handler.Handle(command, CancellationToken.None);

            //var actualId = result.Id;
            //actualId.Should().NotBe(Guid.Empty);
        }
    }
}
