using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Holism.Api
{
    public class Application
    {
        public static void Run()
        {
            var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5000");
            });
            builder.Build().Run();
        }
    }
}