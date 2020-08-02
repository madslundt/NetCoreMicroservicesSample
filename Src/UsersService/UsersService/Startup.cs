using DataModel;
using Events;
using Infrastructure.Consul;
using Infrastructure.Core;
using Infrastructure.EventStores.MongoDb;
using Infrastructure.Logging;
using Infrastructure.MessageBrokers.RabbitMQ;
using Infrastructure.Outbox;
using Infrastructure.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace UsersService
{
    public class Startup
    {
        private readonly IConfigurationRoot Configuration;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            Log.Logger = LoggingExtensions.AddLogging(Configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString(ConnectionStringKeys.App)));

            services
                .AddConsul(Configuration)
                .AddRabbitMQ(Configuration)
                .AddMongoDbEventStore<UserAggregate>(Configuration)
                .AddOutbox(Configuration)
                .AddSwagger(Configuration)
                .AddCore(typeof(Startup), typeof(DatabaseContext)); // Types are needed for mediator to work the different projects. In this case startup is added for this project and DatabaseContext for the DataModel project.
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseLogging(Configuration, loggerFactory)
                .UseSwagger(Configuration)
                .UseConsul(lifetime)
                .UseCore();

            app.UseSubscribeAllEvents();
        }
    }
}
