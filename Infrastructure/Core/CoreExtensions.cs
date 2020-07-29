using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure.Core
{
    public static class CoreExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, Assembly assembly)
        {
            services.AddMediatR(assembly);

            services.AddScoped<TransactionId>();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddOptions();

            services
                .AddMvc(opt => { opt.Filters.Add<ExceptionFilter>(); })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssembly(assembly); });

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
            });

            return app;
        }
    }
}
