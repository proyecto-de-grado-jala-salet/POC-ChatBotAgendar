namespace Services.Interfaces;

public interface IGoogleCalendarService
{
    Task<bool> CreateEventAsync(DateTime startTime, string specialty);
}
