using MediatR;

namespace MicroserviceBase.Hangfire
{
    public class HangfireMediator
    {
        private readonly IMediator _mediator;

        public HangfireMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void SendCommand(IRequest request)
        {
            _mediator.Send(request).Wait();
        }
    }
}
