using System.IO.Pipes;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public class Program
{
    private static string _pipeName = "MeetingNotifierPipe";

    private static void Main(string[] args)
    {
        // Если в аргументах запуска введено "notify", то консоль запустится в режиме получения оповещений о встречах 
        if (args.Length != 0 && args[0] == "notify")
        {
            using var pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.None);
            Console.WriteLine("Wait for main application...");
            // Ждем запуска консоли с основным функционалом
            pipeClient.Connect();
            Console.Clear();
            Console.WriteLine("Meeting Notifications:");
            using var reader = new StreamReader(pipeClient);
            // До тех пор, пока от главной консоли приходят оповещения, цикл будет продолжаться
            while (pipeClient.IsConnected && !reader.EndOfStream)
            {
                Console.WriteLine(reader.ReadLine());
            }
            Console.WriteLine("Disconnect");
        }
        // Иначе консоль запустится с основным функционалом работы со встречами
        else
        {
            var host = CreateHostBuilder(args).Build();
            var app = host.Services.GetRequiredService<App>();
            app.Run();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureServices(services =>
        {
            services
                .AddSingleton<App>()
                .AddSingleton<MeetingService>()
                .AddSingleton<INotifyService>(
                    _ => {
                        return new NotifyService(_pipeName);
                    }
                );
        });
}
