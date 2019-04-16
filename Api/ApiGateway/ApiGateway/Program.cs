using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureKestrel((context, options) =>
                {
                    // Set properties and call methods on options
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
