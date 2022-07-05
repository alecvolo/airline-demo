namespace BuildingBlocks.Helpers;

public static class DateTimeTrimmer
{
    private static readonly TimeSpan OneMinuteInterval = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan OneSecondInterval = TimeSpan.FromSeconds(1);
    public static DateTimeOffset FloorTime(this DateTimeOffset dto, TimeSpan interval)
    {
        return interval.Ticks > 0 ? dto.AddTicks(-dto.Ticks % interval.Ticks) : dto;
    }
    public static DateTime FloorTime(this DateTime dto, TimeSpan interval)
    {
        return interval.Ticks > 0 ? dto.AddTicks(-dto.Ticks % interval.Ticks) : dto;
    }

    public static DateTimeOffset TrimToMinutes(this DateTimeOffset dto) => dto.FloorTime(OneMinuteInterval);
    public static DateTimeOffset TrimToSeconds(this DateTimeOffset dto) => dto.FloorTime(OneSecondInterval);

    public static DateTime TrimToMinutes(this DateTime dto) => dto.FloorTime(OneMinuteInterval);
    public static DateTime TrimToSeconds(this DateTime dto) => dto.FloorTime(OneSecondInterval);
}