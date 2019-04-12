using Base.Hangfire;
using Hangfire;
using MediatR;
using System;

namespace Base.Mediator
{
    public class MediatorJobActivator : JobActivator
    {
        private readonly IMediator _mediator;

        public MediatorJobActivator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override object ActivateJob(Type type)
        {
            return new HangfireMediator(_mediator);
        }
    }
}
