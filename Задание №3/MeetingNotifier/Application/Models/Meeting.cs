namespace Application.Models;

public class Meeting
{
    public Meeting(string name, DateTime start, DateTime end, TimeSpan reminderTime)
    {
        Id = Guid.NewGuid();
        Name = name;
        StartDate = start;
        EndDate = end;
        ReminderTime = reminderTime;
    }

    public Guid Id { get; set; }

    public string Name { get; set;}

    private DateTime _startDate;
    public DateTime StartDate 
    {
        get => _startDate;
        set 
        {
            var nowDate = DateTime.Now;
            if (value > nowDate)
            {
                _startDate = value;
            }
            else
            {
                throw new ArgumentException($"Start date ({value}) is earlier than now date ({nowDate})");
            }
        }
    }

    private DateTime _endDate;
    public DateTime EndDate 
    { 
        get => _endDate;
        set 
        {
            if (value > StartDate)
            {
                _endDate = value;
            }
            else {
                throw new ArgumentException($"End date ({value}) is earlier than start date ({StartDate})");
            }
        }
    }

    private TimeSpan _reminderTime;
    public TimeSpan ReminderTime 
    { 
        get => _reminderTime;
        set
        {
            // TODO: протестить
            if(value.Ticks >= 0)
            {
                _reminderTime = value;
            }
            else
            {
                throw new ArgumentException("Reminder time must be equal or greater than zero");
            }
        }
    }
}