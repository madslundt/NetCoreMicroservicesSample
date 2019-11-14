using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.Configuration.Exchange;
using RawRabbit.vNext;
using RawRabbit.vNext.Pipe;
using System;
using System.Collections.Generic;
using System.Reflection;
using UsersService.Event;
using UsersService.Infrastructure.Filter;
using UsersService.Infrastructure.RabbitMQ;
using UsersService.Pipeline;

namespace UsersService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddRawRabbit();

            services.AddRawRabbit(new RawRabbitOptions
            {
                ClientConfiguration = new RawRabbitConfiguration
                {
                    Username = "guest",
                    Password = "guest",
                    Port = 5672,
                    VirtualHost = "/",
                    Hostnames = { "localhost" },
                    RequestTimeout = TimeSpan.FromSeconds(10),
                    PublishConfirmTimeout = TimeSpan.FromSeconds(1),
                    RecoveryInterval = TimeSpan.FromSeconds(1),
                    PersistentDeliveryMode = true,
                    AutoCloseConnection = true,
                    AutomaticRecovery = true,
                    TopologyRecovery = true,
                    Exchange = new GeneralExchangeConfiguration
                    {
                        Type = ExchangeType.Topic,
                        AutoDelete = false,
                        Durable = true
                    }
                }
            });

            services.AddSingleton(svc => new RabbitEventListener(svc.GetRequiredService<IBusClient>(), svc));

            

            services
                .AddMvc(opt => { opt.Filters.Add(typeof(ExceptionFilter)); })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });


            services.AddControllers()
                .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseRabbitListeners(new List<Type>
            {
                typeof(UserCreated.UserCreatedEvent)
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
