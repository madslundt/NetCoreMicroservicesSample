using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.LoadBalancing.Fabio;
using Convey.Logging;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Metrics.AppMetrics;
using Convey.Persistence.Redis;
using Convey.Tracing.Jaeger;
using Convey.Tracing.Jaeger.RabbitMQ;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Users.Service.Commands;
using Users.Service.Queries;
using static Users.Service.Events.UserCreated;

namespace Users.Service
{
    public class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services => services
                    .AddOpenTracing()
                    .AddConvey()
                    .AddConsul()
                    .AddFabio()
                    .AddJaeger()
                    .AddCommandHandlers()
                    .AddEventHandlers()
                    .AddQueryHandlers()
                    .AddInMemoryCommandDispatcher()
                    .AddInMemoryEventDispatcher()
                    .AddInMemoryQueryDispatcher()
                    .AddRedis()
                    .AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin())
                    .AddMetrics()
                    .AddWebApi()
                    .Build())
                .Configure(app => app
                    .UseErrorHandler()
                    .UseInitializers()
                    .UseRouting()
                    .UseDispatcherEndpoints(endpoints => endpoints
                        .Get("ping", ctx => ctx.Response.WriteAsync("OK"))
                        .Get<GetUser.Query, GetUser.Result>("users/{userId}")
                        .Post<CreateUser.Command>("users", afterDispatch: (cmd, ctx) => ctx.Response.Created($"users/{cmd.UserId}")))
                    .UseJaeger()
                    .UseInitializers()
                    .UseMetrics()
                    .UseRabbitMq()
                    .SubscribeEvent<UserCreatedEvent>())
                .UseLogging();
            });

    }
}
