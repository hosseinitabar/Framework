using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Holism.Api
{
    public class Application
    {
        public static void Run()
        {
            var builder = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                });
            builder.Build().Run();
        }
    }
}