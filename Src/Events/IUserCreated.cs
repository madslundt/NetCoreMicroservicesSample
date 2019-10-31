using Convey.CQRS.Events;
using System;

namespace Events
{
    public interface IUserCreated : IEvent
    {
        public Guid UserId { get; }
    }
}
