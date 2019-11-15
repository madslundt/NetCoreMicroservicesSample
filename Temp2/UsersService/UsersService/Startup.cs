using DataModel;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;
using UsersService.Infrastructure.Filter;
using UsersService.Pipeline;

namespace UsersService
{
    public class Startup
    {
        private readonly IConfigurationRoot Configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var elasticUri = Configuration["ElasticConfiguration:Uri"];

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                })
            .CreateLogger();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString(ConnectionStringKeys.App)));

            //services.AddRawRabbit(new RawRabbitOptions
            //{
            //    ClientConfiguration = new RawRabbitConfiguration
            //    {
            //        Username = "guest",
            //        Password = "guest",
            //        Port = 5672,
            //        VirtualHost = "/",
            //        Hostnames = { "localhost" },
            //        RequestTimeout = TimeSpan.FromSeconds(10),
            //        PublishConfirmTimeout = TimeSpan.FromSeconds(1),
            //        RecoveryInterval = TimeSpan.FromSeconds(1),
            //        PersistentDeliveryMode = true,
            //        AutoCloseConnection = true,
            //        AutomaticRecovery = true,
            //        TopologyRecovery = true,
            //        Exchange = new GeneralExchangeConfiguration
            //        {
            //            Type = ExchangeType.Topic,
            //            AutoDelete = false,
            //            Durable = true
            //        }
            //    }
            //});            

            services
                .AddMvc(opt => { opt.Filters.Add(typeof(ExceptionFilter)); })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });


            services.AddControllers()
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddSerilog();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
