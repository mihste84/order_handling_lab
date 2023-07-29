
namespace Common.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime GetUtc() => DateTime.UtcNow;
}