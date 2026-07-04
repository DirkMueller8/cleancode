namespace Logger.Core;

/// <summary>
/// The filter for the <c>minute</c> disposition (REQ-0011): floors an epoch-seconds timestamp to the
/// start of its containing minute, obscuring precise timing. Pure; needs no pseudonym context. The
/// output stays in canonical epoch-seconds text — rendering it as a clock time is a Viewer concern.
/// (Floor, not "nearest" — a documented, deliberate deviation from DSS §5.)
/// </summary>
public sealed class MinuteFilter : IFieldFilter
{
    private const long SecondsPerMinute = 60;

    public string Apply(FilterInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (!long.TryParse(input.Value, out long epochSeconds))
        {
            throw new ArgumentException(
                $"Value '{input.Value}' is not a valid epoch-seconds timestamp.",
                nameof(input));
        }

        // Sign-safe floor: a plain `epoch % 60` would round toward zero for negative epochs.
        long secondsIntoMinute = ((epochSeconds % SecondsPerMinute) + SecondsPerMinute) % SecondsPerMinute;
        long flooredToMinute = epochSeconds - secondsIntoMinute;
        return flooredToMinute.ToString();
    }
}
