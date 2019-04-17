using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using MediatR;
using MediatR.Pipeline;
using MicroserviceBase.Metrics;
using MicroserviceBase.Validation;
using MicroserviceBase.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using StructureMap;
using Hangfire;
using MicroserviceBase.Hangfire;
using Swashbuckle.AspNetCore.Swagger;
using FluentValidation.AspNetCore;
using Steeltoe.Discovery.Client;
using Base;

namespace MicroserviceBase
{
    public static class Startup
    {
        public static IServiceProvider ConfigureMicroServices<T>(this IServiceCollection services, ConfigureServicesOptions options)
        {
            services.ConfigureServices(options);

            services.AddMediatR(typeof(T));
            services.AddOptions();

            services.AddDiscoveryClient(options.Configuration);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            services.AddMvc(opt => { opt.Filters.Add(typeof(ExceptionFilter)); })
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<T>(); });

            IContainer container = new Container();
            container.Configure(config =>
            {
                config.Populate(services);
            });

            var mediator = container.GetInstance<IMediator>();
            GlobalConfiguration.Configuration.UseMediatR(mediator);


            if (options.HangfireOptions != null)
            {
                services.AddHangfire(options.HangfireOptions);
                services.AddHangfireServer();
            }

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info
            //    {
            //        Version = "v1",
            //        Title = "API",
            //        Description = "API v1",
            //        TermsOfService = "None",
            //    });
            //    c.CustomSchemaIds(x => x.FullName);

            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    c.IncludeXmlComments(xmlPath);
            //});

            return container.GetInstance<IServiceProvider>();
        }

        public static void ConfigureMicro(this IApplicationBuilder app, ConfigureOptions options)
        {
            app.Configure();

            if (options.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            //});

            app.UseDiscoveryClient();

            if (options.HangfireOptions != null)
            {
                app.UseHangfireServer(options.HangfireOptions);

                if (options.HangfireDashboardOptions != null)
                {
                    app.UseHangfireDashboard("/hangfire", options.HangfireDashboardOptions);
                }
            }

            app.UseMvc();
        }
    }

    public class ConfigureServicesOptions : Base.ConfigureServicesOptions
    {
        public Action<IGlobalConfiguration> HangfireOptions { get; set; }
    }

    public class ConfigureOptions
    {
        public DashboardOptions HangfireDashboardOptions { get; set; }
        public BackgroundJobServerOptions HangfireOptions { get; set; }
        public IHostingEnvironment Environment { get; set; }
    }
}
