using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.Logging;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Metrics.AppMetrics;
using Convey.Persistence.Redis;
using Convey.Tracing.Jaeger;
using Convey.Tracing.Jaeger.RabbitMQ;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using DataModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Threading.Tasks;
using UsersService.Commands;
using UsersService.Queries;
using static UsersService.Events.UserCreated;

namespace UsersService
{
    public class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services =>
                {
                    services
                        .AddOpenTracing()
                        .AddConvey()
                        .AddConsul()
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
                        .Build();

                    var builder = new ConfigurationBuilder()
                        .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    var Configuration = builder.Build();

                    services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AppConnectionString")));
                })
                .Configure(app => app
                    .UseErrorHandler()
                    .UseInitializers()
                    .UseRouting()
                    .UseEndpoints(endpoints => endpoints
                        .Get("", ctx => ctx.Response.WriteAsync(Assembly.GetEntryAssembly().GetName().Name))
                        .Get("ping", ctx => ctx.Response.WriteAsync("OK")))
                    .UseDispatcherEndpoints(endpoints => endpoints
                        .Get<GetUser.Query, GetUser.Result>("users/{userId}")
                        .Post<CreateUser.Command>("users", afterDispatch: (cmd, ctx) => ctx.Response.Created($"users/{cmd.UserId}")))
                    .UseJaeger()
                    .UseInitializers()
                    .UseMetrics()
                    .UseRabbitMq()
                    .SubscribeCommand<CreateUser.Command>()
                    .SubscribeEvent<UserCreatedEvent>())
                .UseLogging();
            });
    }
}
