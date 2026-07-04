namespace Logger.Core;

/// <summary>
/// A request to view unfiltered (raw) log data (REQ-0015): who is asking and for which log. The fuller
/// request detail from DSS §3 — the subset of logs, time window, and reason — belongs to the approval
/// workflow (Epic H) and is deliberately out of scope here.
/// </summary>
public sealed class AccessRequest
{
    public string User { get; }

    public string LogTypeName { get; }

    public AccessRequest(string user, string logTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(logTypeName);
        this.User = user;
        this.LogTypeName = logTypeName;
    }
}
