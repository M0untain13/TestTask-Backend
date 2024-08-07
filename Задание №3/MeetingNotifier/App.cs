using System.Text;
using MeetingNotifier.Models;
using MeetingNotifier.Services;

namespace MeetingNotifier;

public class App
{
    private readonly MeetingService _meetingService;

    public App(MeetingService meetingService)
    {
        _meetingService = meetingService;
    }

    public void Run()
    {
        Console.WriteLine("Input command \"help\" for list of commands");
        var isRun = true;
        while (isRun)
        {
            Console.Write("\nCommand: ");
            var command = Console.ReadLine();
            try
            {
                switch (command)
                {
                    case "add":

                        break;
                    case "get":
                        
                        break;
                    case "mod":

                        break;
                    case "del":

                        break;
                    case "help":
                        Console.WriteLine(
                            "add - create meeting\n" +
                            "get - output meetings\n" +
                            "mod - modify meeting\n" +
                            "del - remove meeting\n" +
                            "exit - shutdown this app"
                        );
                        break;
                    case "exit":
                        isRun = false;
                        break;
                    default:
                        Console.WriteLine("Unexpected command");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}