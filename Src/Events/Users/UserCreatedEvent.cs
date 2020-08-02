using Elastic.Apm.Api;
using FluentValidation;
using Infrastructure.Core.Events;
using System;

namespace Events.Users
{
    public class UserCreatedEvent : IEvent
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public class Validator : AbstractValidator<UserCreatedEvent>
        {
            public Validator()
            {
                RuleFor(cmd => cmd.UserId).Empty();
                RuleFor(cmd => cmd.FirstName).Empty();
                RuleFor(cmd => cmd.LastName).Empty();
                RuleFor(cmd => cmd.Email).Empty().EmailAddress();
            }
        }
    }
}
