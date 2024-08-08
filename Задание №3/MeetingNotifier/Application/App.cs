using System.Text;
using Application.Services;

namespace Application;

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
                        AddHandler();
                        break;
                    case "get":
                        GetHandler();
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

    private void AddHandler()
    {
        Console.Write("Name :");
        var name = Console.ReadLine() ?? "Unnamed";
        Console.WriteLine("Example - 31.12.2024 21:00");
        Console.Write("Start date: ");
        var startDate = DateTime.Parse(Console.ReadLine() ?? "");
        Console.Write("End date: ");
        var endDate = DateTime.Parse(Console.ReadLine() ?? "");
        Console.Write("Reminder time (minutes before): ");
        var reminderTime = TimeSpan.FromMinutes(Convert.ToInt64(Console.ReadLine() ?? ""));
        Console.WriteLine(_meetingService.AddMeeting(name, startDate, endDate, reminderTime));
    }

    private void GetHandler()
    {
        Console.WriteLine("Example - 31.12.2024");
        Console.Write("Date: ");
        var date = DateOnly.Parse(Console.ReadLine() ?? "");
        var meetings = _meetingService.GetMeetings(date);
        if (meetings.Any())
        {
            meetings = meetings.OrderBy(m => m.StartDate);
            var sb = new StringBuilder($"Meetings on {date}:");
            foreach (var meeting in meetings)
            {
                sb.Append($"\n{meeting.Name} ({meeting.StartDate}) - ({meeting.EndDate})");
            }
            Console.WriteLine(
                "console - output in console\n" +
                "file - output in file"
            );
            Console.Write("Choice: ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "console":
                    Console.WriteLine(sb);
                    break;
                case "file":
                    Console.Write("File name: ");
                    var fileName = Console.ReadLine();
                    if (fileName is not null)
                    {
                        if(!File.Exists(fileName))
                        {
                            using var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
                            stream.Write(Encoding.UTF8.GetBytes(sb.ToString()));
                            Console.WriteLine("Done");
                        }
                        else
                        {
                            Console.WriteLine($"File with name {fileName} is already exist");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Name must be not empty");
                    }
                    break;
                default:
                    Console.WriteLine("Unexpected choice");
                    break;
            }
        }
        else
        {
            Console.WriteLine("List of meetings is empty");
        }
    }
}