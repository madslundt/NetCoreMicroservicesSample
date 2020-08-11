using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using System;
using System.Collections.Generic;

namespace ApiGateway
{
    public class Startup
    {

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOcelot(Configuration)
                .AddConsul()
                .AddConfigStoredInConsul();

            services.AddSwaggerForOcelot(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.DownstreamSwaggerHeaders = new[]
                {
                    new KeyValuePair<string, string>("x-correlation-id", Guid.NewGuid().ToString()),
                };
            })
            .UseOcelot()
            .Wait();
        }
    }
}
