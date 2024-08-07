using MeetingNotifier.Models;

namespace MeetingNotifier.Services;

public class MeetingService
{
    public MeetingService()
    {
        _meetings = new List<Meeting>();
    }

    private readonly ICollection<Meeting> _meetings;
    private Meeting? meetingWithLatestEnd;

    public string AddMeeting(string name, DateTime start, DateTime end, TimeSpan reminderTime)
    {
        var meeting = new Meeting(name, start, end, reminderTime);
        if (meetingWithLatestEnd is null || meeting.StartDate > meetingWithLatestEnd.EndDate)
        {
            _meetings.Add(meeting);
            return "Done";
        }
        else
        {
            return $"Start date ({meeting.StartDate}) is earlier than latest end date ({meetingWithLatestEnd.EndDate})";
        }
    }

    public IEnumerable<Meeting> GetMeetings(DateOnly date)
    {
        return _meetings.Where(m => m.StartDate.Date.Equals(date) || m.EndDate.Date.Equals(date));
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
                meeting.StartDate = (DateTime)startDate;
            }
            if (endDate is not null)
            {
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
            return "Done";
        }
        else
        {
            return $"Meeting with id {id} not found";
        }
    }
}