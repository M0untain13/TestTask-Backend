using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public class Program
{
    private static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var app = host.Services.GetRequiredService<App>();
        app.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureServices(services => { 
            services
                .AddSingleton<App>()
                .AddSingleton<MeetingService>();
        });
}
