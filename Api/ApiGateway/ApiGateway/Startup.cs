using System.Text;
using ApiGateway.GraphQL;
using Base;
using GQL = GraphQL;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using System;

namespace ApiGateway
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile(path: "routes.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            _logger = logger;
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        private readonly ILogger<Startup> _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // GraphQL
            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddGraphQL(o =>
            {
                o.ExposeExceptions = true;
                o.ComplexityConfiguration = new GQL.Validation.Complexity.ComplexityConfiguration { MaxDepth = 15 };
            })
            .AddGraphTypes(ServiceLifetime.Singleton);
            services.AddSingleton<ISchema, RootSchema>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddOcelot()
                //.AddDelegatingHandler<GraphQLDelegatingHandler>()
                .AddConsul();

            //services.AddCors();

            //var key = Encoding.ASCII.GetBytes("THIS_IS_A_RANDOM_SECRET_2e7a1e80-16ee-4e52-b5c6-5e8892453459");

            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer("ApiSecurity", x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});

            return services.ConfigureServices(new ConfigureServicesOptions
            {
                Configuration = Configuration,
                Logger = _logger
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Configure();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                //app.UseCors(b => b
                //    .AllowAnyOrigin()
                //    .AllowAnyMethod()
                //    .AllowAnyHeader()
                //    .AllowCredentials()
                //);
            }

            app.UseGraphQL<Schema>();
            app.UseOcelot();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
