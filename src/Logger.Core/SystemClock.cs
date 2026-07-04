namespace Logger.Core;

/// <summary>The production <see cref="IClock"/>: the real system clock in UTC.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
