using Application.Services;
using Xunit;

namespace Tests;

public class MeetingServiceTest
{
    [Fact]
    // Проверка на правильное добавление и получение встреч в определенные дни
    public void AddAndGet()
    {
        var meetingService = new MeetingService();
        meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("31.12.2024 23:00"),
            DateTime.Parse("01.01.2025 06:00"),
            TimeSpan.FromMinutes(0)
        );
        meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("31.12.2024 10:00"),
            DateTime.Parse("31.12.2024 22:00"),
            TimeSpan.FromMinutes(0)
        );
        meetingService.AddMeeting(
            "Name3",
            DateTime.Parse("25.12.2024 16:00"),
            DateTime.Parse("25.12.2024 18:00"),
            TimeSpan.FromMinutes(0)
        );

        var meetings1 = meetingService.GetMeetings(DateOnly.Parse("31.12.2024"));
        var meetings2 = meetingService.GetMeetings(DateOnly.Parse("25.12.2024"));
        var meetings3 = meetingService.GetMeetings(DateOnly.Parse("15.02.2023"));

        Assert.Equal(2, meetings1.Count());
        Assert.Single(meetings2);
        Assert.Empty(meetings3);
    }

    [Fact]
    // Проверка на возникновение ошибки о пересечении встреч
    public void IntersectError1()
    {
        var meetingService = new MeetingService();

        var message1 = meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("01.01.2025 04:00"),
            DateTime.Parse("01.01.2025 06:00"),
            TimeSpan.FromMinutes(0)
            );
        var message2 = meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("01.01.2025 05:00"),
            DateTime.Parse("01.01.2025 07:00"),
            TimeSpan.FromMinutes(0)
        );

        Assert.Equal("Done", message1);
        Assert.NotEqual("Done", message2);
    }

    [Fact]
    // Проверка на возникновение ошибки о пересечении встреч
    public void IntersectError2()
    {
        var meetingService = new MeetingService();

        var message1 = meetingService.AddMeeting(
            "Name1",
            DateTime.Parse("01.01.2025 04:00"),
            DateTime.Parse("01.01.2025 07:00"),
            TimeSpan.FromMinutes(0)
            );
        var message2 = meetingService.AddMeeting(
            "Name2",
            DateTime.Parse("01.01.2025 04:00"),
            DateTime.Parse("01.01.2025 05:00"),
            TimeSpan.FromMinutes(0)
        );

        Assert.Equal("Done", message1);
        Assert.NotEqual("Done", message2);
    }
}