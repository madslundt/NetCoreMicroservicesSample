using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;

namespace Infrastructure.Logging
{
    public static class LoggingExtensions
    {
        public static Serilog.Core.Logger AddLogging(IConfiguration Configuration)
        {            
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .ReadFrom.Configuration(Configuration)
            .CreateLogger();

            return logger;
        }

        public static void UseLogging(this IApplicationBuilder app, IConfiguration Configuration)
        {
            app.UseAllElasticApm(Configuration);
        }

        public static void UseLogging(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
        }
    }
}
