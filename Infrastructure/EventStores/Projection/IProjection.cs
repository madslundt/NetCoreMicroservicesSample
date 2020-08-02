using Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EventStores.Projection
{
    public interface IProjection
    {
        Type[] Handles { get; }
        void Handle(IEvent @event);
    }
}
