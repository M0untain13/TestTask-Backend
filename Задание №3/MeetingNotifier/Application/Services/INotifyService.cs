using Application.Models;

namespace Application.Services;

public interface INotifyService
{
    void AddMeeting(Meeting meeting);
    void RemoveMeeting(Meeting meeting);
    void Run();
    void Stop();
}