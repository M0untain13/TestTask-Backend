using Application.Services;
using Microsoft.VisualBasic;
using Xunit;

namespace Tests;

public class MeetingServiceTest
{
    [Fact]
    // Проверка на правильное добавление встреч
    public void AddTest()
    {
        var meetingService = new MeetingService();

        var messageAboutCreate1 = meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("31.12.3024 23:00"),
            DateTime.Parse("01.01.3025 06:00"),
            TimeSpan.FromMinutes(0)
        );
        var messageAboutCreate2 = meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("31.12.3024 10:00"),
            DateTime.Parse("31.12.3024 22:00"),
            TimeSpan.FromMinutes(0)
        );
        var messageAboutCreate3 = meetingService.AddMeeting(
            "Name3",
            DateTime.Parse("25.12.3024 16:00"),
            DateTime.Parse("25.12.3024 18:00"),
            TimeSpan.FromMinutes(0)
        );

        Assert.Equal("Done", messageAboutCreate1);
        Assert.Equal("Done", messageAboutCreate2);
        Assert.Equal("Done", messageAboutCreate3);
    }

    [Fact]
    // Проверка на правильное получение встреч в определенные дни
    public void GetTest()
    {
        var meetingService = new MeetingService();
        meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("31.12.3024 23:00"),
            DateTime.Parse("01.01.3025 06:00"),
            TimeSpan.FromMinutes(0)
        );
        meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("31.12.3024 10:00"),
            DateTime.Parse("31.12.3024 22:00"),
            TimeSpan.FromMinutes(0)
        );
        meetingService.AddMeeting(
            "Name3",
            DateTime.Parse("25.12.3024 16:00"),
            DateTime.Parse("25.12.3024 18:00"),
            TimeSpan.FromMinutes(0)
        );

        var meetings1 = meetingService.GetMeetings(DateOnly.Parse("31.12.3024"));
        var meetings2 = meetingService.GetMeetings(DateOnly.Parse("25.12.3024"));
        var meetings3 = meetingService.GetMeetings(DateOnly.Parse("15.02.3023"));

        Assert.Equal(2, meetings1.Count());
        Assert.Single(meetings2);
        Assert.Empty(meetings3);
    }

    [Fact]
    // Проверка на правильную модификацию встреч
    public void ModifyTest()
    {
        var meetingService = new MeetingService();
        meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("31.12.3024 23:00"),
            DateTime.Parse("01.01.3025 06:00"),
            TimeSpan.FromMinutes(0)
        );
        var newName = "Name2";
        var newStartDate = DateTime.Parse("01.01.3025 23:00");
        var newEndDate = DateTime.Parse("01.02.3025 23:00");
        var newReminderTime = TimeSpan.FromMinutes(30);

        var meeting = meetingService.GetMeetings(DateOnly.Parse("31.12.3024")).First();
        var messageAboutMod1 = meetingService.ModifyMeeting(meeting.Id, newName, newStartDate, newEndDate, newReminderTime);
        meeting = meetingService.GetMeetings(DateOnly.Parse("01.01.3025")).First();

        Assert.Equal("Done", messageAboutMod1);
        Assert.Equal(newName, meeting.Name);
        Assert.Equal(newStartDate, meeting.StartDate);
        Assert.Equal(newEndDate, meeting.EndDate);
        Assert.Equal(newReminderTime, meeting.ReminderTime);
    }

    [Fact]
    // Проверка на возникновение ошибки о пересечении встреч при добавлении
    public void IntersectErrorTest1()
    {
        var meetingService = new MeetingService();

        var message1 = meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("01.01.3025 04:00"),
            DateTime.Parse("01.01.3025 06:00"),
            TimeSpan.FromMinutes(0)
            );
        var message2 = meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("01.01.3025 05:00"),
            DateTime.Parse("01.01.3025 07:00"),
            TimeSpan.FromMinutes(0)
        );

        Assert.Equal("Done", message1);
        Assert.NotEqual("Done", message2);
    }

    [Fact]
    // Проверка на возникновение ошибки о пересечении встреч при добавлении
    public void IntersectErrorTest2()
    {
        var meetingService = new MeetingService();

        var message1 = meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("01.01.3025 04:00"),
            DateTime.Parse("01.01.3025 07:00"),
            TimeSpan.FromMinutes(0)
            );
        var message2 = meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("01.01.3025 04:00"),
            DateTime.Parse("01.01.3025 05:00"),
            TimeSpan.FromMinutes(0)
        );

        Assert.Equal("Done", message1);
        Assert.NotEqual("Done", message2);
    }

    [Fact]
    // Проверка на возникновение ошибки о пересечении встреч при модицикации
    public void IntersectErrorTest3()
    {
        var meetingService = new MeetingService();

        var messageAboutCreate1 = meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("01.01.3025 04:00"),
            DateTime.Parse("01.01.3025 06:00"),
            TimeSpan.FromMinutes(0)
            );
        var messageAboutCreate2 = meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("01.01.3025 07:00"),
            DateTime.Parse("01.01.3025 09:00"),
            TimeSpan.FromMinutes(0)
        );
        var meetings = meetingService.GetMeetings(DateOnly.Parse("01.01.3025"));
        var meeting1 = meetings.OrderBy(m => m.StartDate).First();
        var messageAboutModify = meetingService.ModifyMeeting(meeting1.Id, endDate: DateTime.Parse("01.01.3025 08:00"));

        Assert.Equal("Done", messageAboutCreate1);
        Assert.Equal("Done", messageAboutCreate2);
        Assert.NotEqual("Done", messageAboutModify);
    }

    [Fact]
    // Проверка на правильность удаления встречи
    public void DeleteTest()
    {
        var meetingService = new MeetingService();
        meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("01.01.3025 04:00"),
            DateTime.Parse("01.01.3025 06:00"),
            TimeSpan.FromMinutes(0)
            );
        meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("01.01.3025 07:00"),
            DateTime.Parse("01.01.3025 09:00"),
            TimeSpan.FromMinutes(0)
        );

        var meetings = meetingService.GetMeetings(DateOnly.Parse("01.01.3025"));
        var countBeforeDelete = meetings.Count();
        var meeting = meetings.First();
        var deleteMessage = meetingService.DeleteMeeting(meeting.Id);
        meetings = meetingService.GetMeetings(DateOnly.Parse("01.01.3025"));
        var countAfterDelete = meetings.Count();

        Assert.Equal(2, countBeforeDelete);
        Assert.Equal("Done", deleteMessage);
        Assert.Equal(1, countAfterDelete);
    }
}