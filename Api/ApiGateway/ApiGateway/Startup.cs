using Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using System;

namespace ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration config, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _logger = logger;
            _env = env;
            _config = config;
        }

        public readonly IConfiguration _config;
        private readonly ILogger<Startup> _logger;
        private readonly IHostingEnvironment _env;

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.ConfigureServices(new ConfigureServicesOptions
            {
                Configuration = _config,
                Logger = _logger
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Configure();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
                app.UseCors(b => b
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            }

            app.UseOcelot();
        }
    }
}
