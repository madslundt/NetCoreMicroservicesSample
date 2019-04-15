using MicroserviceBase.Hangfire;
using Hangfire;
using MediatR;

namespace MicroserviceBase.Mediator
{
    public static class MediatorExtension
    {
        public static void Enqueue(this IMediator mediator, IRequest request)
        {
            BackgroundJob.Enqueue<HangfireMediator>(m => m.SendCommand(request));
        }
    }
}
