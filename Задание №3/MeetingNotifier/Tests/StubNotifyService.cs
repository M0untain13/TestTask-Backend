using Application.Models;
using Application.Services;

namespace Tests;

public class StubNotifyService : INotifyService
{
    public void Run() { }

    public void Stop() { }

    public void AddMeeting(Meeting meeting) { }

    public void RemoveMeeting(Meeting meeting) { }
}
