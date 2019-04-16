using App.Metrics;
using App.Metrics.Reporting.InfluxDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Linq;
using System.Reflection;

namespace Base
{
    public static class Startup
    {
        public static IServiceProvider ConfigureServices(this IServiceCollection services, ConfigureServicesOptions options)
        {
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

            IContainer container = new Container();
            container.Configure(config =>
            {
                config.Populate(services);
            });

            services.AddMvc().AddMetrics();

            metrics.ReportRunner.RunAllAsync();

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

            services.AddLogging(builder => builder
                .AddConfiguration(options.Configuration)
                .AddConsole()
                .AddDebug()
                .AddSentry());

            return container.GetInstance<IServiceProvider>();
        }

        public static void Configure(this IApplicationBuilder app)
        {
            app.UseMetricsAllEndpoints();
            app.UseMetricsAllMiddleware();

            app.UseStaticFiles();
        }
    }

    public class ConfigureServicesOptions
    {
        public IConfiguration Configuration { get; set; }
        public ILogger Logger { get; set; }
    }
}
