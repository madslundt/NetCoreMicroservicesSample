using System;
using ApiGraphQL.Infrastructure.GraphQL;
using Base;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.Discovery.Client;

namespace ApiGraphQL
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
            // GraphQL
            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddGraphQL(o =>
            {
                o.ExposeExceptions = true;
                o.ComplexityConfiguration = new GraphQL.Validation.Complexity.ComplexityConfiguration { MaxDepth = 15 };
            })
            .AddGraphTypes(ServiceLifetime.Singleton);
            services.AddSingleton<ISchema, RootSchema>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



            return services.ConfigureServices(new ConfigureServicesOptions
            {
                Configuration = _config,
                Logger = _logger
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
            }

            app.UseMiddleware<GraphQLMiddleware>(new GraphQLSettings
            {
                BuildUserContext = ctx => new GraphQLUserContext
                {
                    User = ctx.User
                }
            });

            app.UseDiscoveryClient();

            app.UseStaticFiles();

            app.UseGraphQL<Schema>();
        }
    }
}
