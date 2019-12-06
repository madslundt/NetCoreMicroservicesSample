using AutoFixture.Xunit2;
using FluentAssertions;
using FluentValidation;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using UsersService.Commands;
using Xunit;

namespace UnitTest.Commands
{
    public class CreateUserValidationTest : TestBase
    {
        [Theory]
        [InlineAutoData("")]
        [InlineAutoData(null)]
        public void Should_throw_ValidationException_When_FirstName_is_empty(string firstName, CreateUser.Command command, MailAddress mailAddress)
        {
            command.FirstName = firstName;
            command.Email = mailAddress.Address;

            Func<Task> act = async () => await _mediator.Send(command);

            act.Should().Throw<ValidationException>();
        }

        [Theory]
        [InlineAutoData("")]
        [InlineAutoData(null)]
        public void Should_throw_ValidationException_When_LastName_is_empty(string lastName, CreateUser.Command command, MailAddress mailAddress)
        {
            command.LastName = lastName;
            command.Email = mailAddress.Address;

            Func<Task> act = async () => await _mediator.Send(command);

            act.Should().Throw<ValidationException>();
        }

        [Theory]
        [InlineAutoData("")]
        [InlineAutoData(null)]
        public void Should_throw_ValidationException_When_Email_is_empty(string email, CreateUser.Command command)
        {
            command.Email = email;

            Func<Task> act = async () => await _mediator.Send(command);

            act.Should().Throw<ValidationException>();
        }

        [Theory]
        [InlineAutoData()]
        public void Should_throw_ValidationException_When_Email_is_invalid(string email, CreateUser.Command command)
        {
            command.Email = email;

            Func<Task> act = async () => await _mediator.Send(command);

            act.Should().Throw<ValidationException>();
        }
    }
}
