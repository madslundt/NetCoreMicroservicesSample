using FluentValidation.AspNetCore;
using Infrastructure.Core.Commands;
using Infrastructure.Core.Events;
using Infrastructure.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Core
{
    public static class CoreExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, params Type[] types)
        {
            var assemblies = types.Select(type => type.GetTypeInfo().Assembly);

            foreach (var assembly in assemblies)
            {
                services.AddMediatR(assembly);
            }

            services.AddScoped<ICommandBus, CommandBus>();
            services.AddScoped<IQueryBus, QueryBus>();
            services.AddScoped<IEventBus, EventBus>();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddOptions();

            services
                .AddMvc(opt => { opt.Filters.Add<ExceptionFilter>(); })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblies(assemblies); });

            services.AddHealthChecks();

            services.AddControllers()
                .AddNewtonsoftJson();


            return services;
        }

        public static IApplicationBuilder UseCore(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            return app;
        }
    }
}
