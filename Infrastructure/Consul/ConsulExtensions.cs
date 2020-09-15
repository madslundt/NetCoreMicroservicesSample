using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Infrastructure.Consul
{
    public static class ConsulExtensions
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new ConsulOptions();
            Configuration.GetSection(nameof(ConsulOptions)).Bind(options);
            services.Configure<ConsulOptions>(Configuration.GetSection(nameof(ConsulOptions)));

            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = options.ConsulAddress;
                consulConfig.Address = new Uri(address);
            }));

            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            // Retrieve Consul client from DI
            var consulClient = app.ApplicationServices
                                .GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices
                                .GetRequiredService<IOptions<ConsulOptions>>();
            // Setup logger
            var loggingFactory = app.ApplicationServices
                                .GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            // Get server IP address
            var address = consulConfig.Value.ServiceAddress;

            if (string.IsNullOrWhiteSpace(address))
            {
                var features = app.Properties["server.Features"] as FeatureCollection;
                var addresses = features.Get<IServerAddressesFeature>();
                address = addresses.Addresses.First();

                Console.WriteLine($"Could not find service address in config. Using '{address}'");
            }

            // Register service with consul
            var uri = new Uri(address);
            var serviceName = consulConfig.Value.Name ?? AppDomain.CurrentDomain.FriendlyName.Trim().Trim('_');
            var registration = new AgentServiceRegistration
            {
                ID = $"{serviceName.ToLowerInvariant()}-{consulConfig.Value.Id ?? Guid.NewGuid().ToString()}",
                Name = serviceName,
                Address = uri.Host,
                Port = uri.Port,
                Tags = consulConfig.Value.Tags
            };

            if (!consulConfig.Value.DisableAgentCheck)
            {
                registration.Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(uri, "health").OriginalString
                };
            }

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
