using System.Text;
using Application.Models;
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
                        ModHandler();
                        break;
                    case "del":
                        DelHandler();
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
        Console.Write("Name: ");
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
        var selectedDate = SelectDate();
        if (selectedDate is null)
        {
            return;
        }
        var meetings = _meetingService.GetMeetings((DateOnly)selectedDate).OrderBy(m => m.StartDate);
        if (!meetings.Any())
        {
            Console.WriteLine($"List of meetings in date {selectedDate} is empty");
            return;
        }

        var sb = new StringBuilder($"Meetings on {selectedDate}:");
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
                    if (!File.Exists(fileName))
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

    private void ModHandler()
    {
        var selectedDate = SelectDate();
        if (selectedDate is null)
        {
            return;
        }
        var selectedMeeting = SelectMeeting((DateOnly)selectedDate);
        if (selectedMeeting is null)
        {
            return;
        }
        Console.WriteLine(
            "name - change name\n" +
            "start - change start date\n" +
            "end - change end date\n" +
            "reminder - change reminder time"
        );
        Console.Write("Choice: ");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "name":
                Console.Write("New name: ");
                var name = Console.ReadLine() ?? "Unnamed";
                Console.WriteLine(_meetingService.ModifyMeeting(selectedMeeting.Id, name: name));
                break;
            case "start":
                Console.Write("New start date: ");
                var startDate = DateTime.Parse(Console.ReadLine() ?? "");
                Console.WriteLine(_meetingService.ModifyMeeting(selectedMeeting.Id, startDate: startDate));
                break;
            case "end":
                Console.Write("New end date: ");
                var endDate = DateTime.Parse(Console.ReadLine() ?? "");
                Console.WriteLine(_meetingService.ModifyMeeting(selectedMeeting.Id, endDate: endDate));
                break;
            case "reminder":
                Console.Write("New reminder time: ");
                var reminderTime = TimeSpan.FromMinutes(Convert.ToInt64(Console.ReadLine() ?? ""));
                Console.WriteLine(_meetingService.ModifyMeeting(selectedMeeting.Id, reminderTime: reminderTime));
                break;
            default:
                Console.WriteLine("Unexpected choice");
                break;
        }
    }

    private void DelHandler()
    {
        var selectedDate = SelectDate();
        if (selectedDate is null)
        {
            return;
        }
        var selectedMeeting = SelectMeeting((DateOnly)selectedDate);
        if (selectedMeeting is null)
        {
            return;
        }
        Console.WriteLine(_meetingService.DeleteMeeting(selectedMeeting.Id));
    }

    private DateOnly? SelectDate()
    {
        var dates = _meetingService.GetAllStartDates().ToArray();
        var count = dates.Length;
        if (count == 0)
        {
            Console.WriteLine("List of meetings is empty");
            return null;
        }

        var sb = new StringBuilder("Meeting dates:");
        for (var i = 0; i < count; i++)
        {
            sb.Append($"\n#{i + 1} {dates[i]}");
        }
        Console.WriteLine(sb);
        Console.Write("Number: ");
        var number = Convert.ToInt64(Console.ReadLine() ?? "");
        return dates[number - 1];
    }

    private Meeting? SelectMeeting(DateOnly date)
    {
        var meetings = _meetingService.GetMeetings(date).OrderBy(m => m.StartDate).ToArray();
        var count = meetings.Length;
        if (count == 0)
        {
            Console.WriteLine($"List of meetings in date {date} is empty");
            return null;
        }

        var sb = new StringBuilder($"Meetings on {date}:");
        for (var i = 0; i < count; i++)
        {
            sb.Append($"\n#{i + 1} {meetings[i].Name} ({meetings[i].StartDate}) - ({meetings[i].EndDate})");
        }
        Console.WriteLine(sb);
        Console.Write("Number: ");
        var number = Convert.ToInt64(Console.ReadLine() ?? "");
        return meetings[number - 1];
    }
}