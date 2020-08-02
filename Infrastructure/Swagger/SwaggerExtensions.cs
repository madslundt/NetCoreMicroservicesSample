using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace Infrastructure.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(options);
            services.Configure<SwaggerOptions>(Configuration.GetSection(nameof(SwaggerOptions)));

            if (string.IsNullOrWhiteSpace(options.Title))
            {
                options.Title = AppDomain.CurrentDomain.FriendlyName.Trim().Trim('_');
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.VersionName, options);
                c.CustomSchemaIds(x => x.FullName);
            });

            return services;
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration Configuration)
        {
            var options = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(options);

            if (string.IsNullOrWhiteSpace(options.Title))
            {
                options.Title = AppDomain.CurrentDomain.FriendlyName.Trim().Trim('_');
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.VersionName}/swagger.json", options.Title);
                c.RoutePrefix = options.RoutePrefix;
            });

            return app;
        }
    }
}
