using Infrastructure.Core.Events;
using System;

namespace Infrastructure.EventStores.Projection
{
    public interface IProjection
    {
        Type[] Handles { get; }
        void Handle(IEvent @event);
    }
}
