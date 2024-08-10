using Application.Models;

namespace Application.Services;

public class MeetingService
{
    private readonly INotifyService _notifyService;
    private readonly ICollection<Meeting> _meetings;

    public MeetingService(INotifyService notifyService)
    {
        _notifyService = notifyService;
        _meetings = new List<Meeting>();
        _notifyService.Run();
    }
    
    private bool CheckIntersect(Guid id, DateTime startDate, DateTime endDate) =>
        _meetings.Any(m => (
            startDate >= m.StartDate && startDate <= m.EndDate
            || endDate >= m.StartDate && endDate <= m.EndDate
            || m.StartDate >= startDate && m.StartDate <= endDate
            || m.EndDate >= startDate && m.EndDate <= endDate
            )
            && m.Id != id);

    public string AddMeeting(string name, DateTime startDate, DateTime endDate, TimeSpan reminderTime)
    {
        if (CheckIntersect(Guid.Empty, startDate, endDate))
        {
            return $"Meetings intersect";
        }
        else
        {
            var meeting = new Meeting(name, startDate, endDate, reminderTime);
            _meetings.Add(meeting);
            _notifyService.AddMeeting(meeting);
            return "Done";
        }
    }

    public IEnumerable<Meeting> GetMeetings(DateOnly date)
    {
        return _meetings.Where(m =>
            DateOnly.FromDateTime(m.StartDate).Equals(date));
    }

    public IEnumerable<DateOnly> GetAllStartDates()
    {
        return _meetings.Select(m => DateOnly.FromDateTime(m.StartDate));
    }

    public string ModifyMeeting(Guid id, string? name = null, DateTime? startDate = null, DateTime? endDate = null, TimeSpan? reminderTime = null)
    {
        var meeting = _meetings.FirstOrDefault(m => m.Id.Equals(id));
        if (meeting is not null)
        {
            if (name is not null)
            {
                meeting.Name = name;
            }
            if (startDate is not null)
            {
                if (CheckIntersect(id, (DateTime)startDate, meeting.EndDate))
                {
                    return $"Meetings intersect";
                }
                meeting.StartDate = (DateTime)startDate;
            }
            if (endDate is not null)
            {
                if (CheckIntersect(id, meeting.StartDate, (DateTime)endDate))
                {
                    return $"Meetings intersect";
                }
                meeting.EndDate = (DateTime)endDate;
            }
            if (reminderTime is not null)
            {
                meeting.ReminderTime = (TimeSpan)reminderTime;
            }
            return "Done";
        }
        else
        {
            return $"Meeting with id {id} not found";
        }
    }

    public string DeleteMeeting(Guid id)
    {
        var meeting = _meetings.FirstOrDefault(m => m.Id.Equals(id));
        if (meeting is not null)
        {
            _meetings.Remove(meeting);
            _notifyService.RemoveMeeting(meeting);
            return "Done";
        }
        else
        {
            return $"Meeting with id {id} not found";
        }
    }
}