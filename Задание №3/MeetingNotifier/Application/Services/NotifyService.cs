using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using Application.Models;

namespace Application.Services;

public class NotifyService : INotifyService
{
    private readonly LinkedList<Meeting> _meetings;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly string _pipeName;

    public NotifyService(string pipeName)
    {
        _meetings = new LinkedList<Meeting>();
        _cancellationTokenSource = new CancellationTokenSource();
        _pipeName = pipeName;
    }

    public void AddMeeting(Meeting meeting)
    {
        // Если список пустой, то просто вставляем в начало
        if (_meetings.Count == 0)
        {
            _meetings.AddFirst(meeting);
            meeting.PropertyChanged += OnMeetingPropertyChanged;
            return;
        }

        // Если в списке есть элементы, то ищем элемент, у которого напоминание позже, чем у добавляемой встречи
        LinkedListNode<Meeting>? next;
        while ((next = _meetings.First) is not null)
        {
            if (next.Value.StartDate.Subtract(next.Value.ReminderTime)
                > meeting.StartDate.Subtract(meeting.ReminderTime))
            {
                // Добавляем встречу перед той, про которую нужно напомнить позже
                _meetings.AddBefore(next, meeting);
                meeting.PropertyChanged += OnMeetingPropertyChanged;
                return;
            }
        }

        // Если добавляется встреча с самой поздней датой напоминания, то вставляем в конец
        _meetings.AddLast(meeting);
        meeting.PropertyChanged += OnMeetingPropertyChanged;
    }

    public void RemoveMeeting(Meeting meeting)
    {
        _meetings.Remove(meeting);
        meeting.PropertyChanged -= OnMeetingPropertyChanged;
    }

    public void Run()
    {
        var process = Process.GetCurrentProcess();
        var pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1);
        Console.WriteLine("Wait for notifications console...");
        // Ждем запуск консоли, которая будет получать оповещения о встречах
        pipeServer.WaitForConnection();
        Console.Clear();
        Task.Run(async () =>
        {
            using var writer = new StreamWriter(pipeServer);
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            // Каждые 5 секунд проверяем, подошли время для оповещения о встрече
            while (!token.IsCancellationRequested)
            {
                // Проверяем только первую встречу, т.к. связный список сортируется по принципу: первая встреча та, о которой нужно раньше всех напомнить
                var meeting = _meetings.First?.Value;
                if (meeting is not null && DateTime.Now > meeting.StartDate.Subtract(meeting.ReminderTime))
                {
                    _meetings.Remove(meeting);
                    writer.WriteLine($"Meeting \"{meeting.Name}\" will start {meeting.StartDate}");
                    writer.Flush();
                }
                await Task.Delay(5000, token);
            }
            process.Close();
            pipeServer.Close();
        });

    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    // Метод вызывается только в тех случаях, когда у встреч обновляются такие данные как начало встречи или время до встречи, за которое нужно напомнить
    // Метод сортирует связный список
    private void OnMeetingPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(sender is Meeting meeting)
        {
            RemoveMeeting(meeting);
            AddMeeting(meeting);
        }
    }
}