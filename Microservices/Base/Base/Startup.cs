using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using MediatR;
using MediatR.Pipeline;
using App.Metrics;
using Microsoft.Extensions.Configuration;
using App.Metrics.Reporting.InfluxDB;
using Base.Metrics;
using Base.Validation;
using Base.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using StructureMap;
using Hangfire;
using Base.Hangfire;
using Swashbuckle.AspNetCore.Swagger;
using FluentValidation.AspNetCore;

namespace Base
{
    public static class Startup
    {
        public static IServiceProvider ConfigureServices(this IServiceCollection services, Type type, ConfigureServicesOptions options)
        {
            services.AddMediatR(typeof(Startup));
            services.AddOptions();

            var metricsConfigSection = options.Configuration.GetSection(nameof(MetricsOptions));
            var influxOptions = new MetricsReportingInfluxDbOptions();
            options.Configuration.GetSection(nameof(MetricsReportingInfluxDbOptions)).Bind(influxOptions);

            var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(metricsConfigSection.AsEnumerable())
                .Report.ToInfluxDb(influxOptions)
                .Build();

            services.AddMetrics(metrics);
            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints();
            services.AddMetricsReportingHostedService();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            services.AddMvc(opt => { opt.Filters.Add(typeof(ExceptionFilter)); })
                .AddMetrics()
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining(type); });

            IContainer container = new Container();
            container.Configure(config =>
            {
                config.Populate(services);
            });

            var mediator = container.GetInstance<IMediator>();
            GlobalConfiguration.Configuration.UseMediatR(mediator);

            metrics.ReportRunner.RunAllAsync();


            if (options.HangfireOptions != null)
            {
                services.AddHangfire(options.HangfireOptions);
                services.AddHangfireServer();
            }

            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t))
                .ToList();

            var sp = services.BuildServiceProvider();
            foreach (var controllerType in controllers)
            {
                options.Logger.LogInformation($"Found {controllerType.Name}");
                try
                {
                    sp.GetService(controllerType);
                }
                catch (Exception ex)
                {
                    options.Logger.LogCritical(ex, $"Cannot create instance of controller {controllerType.FullName}, it is missing some services");
                }
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "API",
                    Description = "API v1",
                    TermsOfService = "None",
                });
                c.CustomSchemaIds(x => x.FullName);

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddLogging(builder => builder
                .AddConfiguration(options.Configuration)
                .AddConsole()
                .AddDebug()
                .AddSentry());

            return container.GetInstance<IServiceProvider>();
        }

        public static void Configure(this IApplicationBuilder app, ConfigureOptions options)
        {
            if (options.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();
            app.UseMetricsAllEndpoints();
            app.UseMetricsAllMiddleware();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });

            if (options.HangfireOptions != null)
            {
                app.UseHangfireServer(options.HangfireOptions);

                if (options.HangfireDashboardOptions != null)
                {
                    app.UseHangfireDashboard("/hangfire", options.HangfireDashboardOptions);
                }
            }
        }
    }

    public class ConfigureServicesOptions
    {
        public Action<IGlobalConfiguration> HangfireOptions { get; set; }
        public IConfigurationRoot Configuration { get; set; }
        public ILogger Logger { get; set; }
    }

    public class ConfigureOptions
    {
        public DashboardOptions HangfireDashboardOptions { get; set; }
        public BackgroundJobServerOptions HangfireOptions { get; set; }
        public IHostingEnvironment Environment { get; set; }
    }
}
