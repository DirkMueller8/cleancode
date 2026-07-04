using Logger.Core;
using static Logger.Services.Tests.Build;

namespace Logger.Services.Tests;

/// <summary>
/// Tests for the session lifecycle (REQ-0022), token issuance (REQ-0023), and token invalidation
/// (REQ-0024).
/// </summary>
public sealed class LoggingSessionTests
{
    private static LoggingSession NewSession(out InMemoryLogStore store)
    {
        store = new InMemoryLogStore();
        return new LoggingSession(new StubTokenGenerator(), store);
    }

    // REQ-0022/0023: Hello is first, issues a token, activates the session.
    [Fact]
    public void Hello_IssuesToken_AndActivates()
    {
        LoggingSession session = NewSession(out _);

        string token = session.Hello();

        Assert.Equal("TEST-TOKEN", token);
        Assert.True(session.IsActive);
        Assert.Equal("TEST-TOKEN", session.Token);
    }

    // REQ-0022: a second Hello is rejected.
    [Fact]
    public void Hello_Twice_IsRejected()
    {
        LoggingSession session = NewSession(out _);
        session.Hello();

        Assert.Throws<InvalidOperationException>(() => session.Hello());
    }

    // REQ-0022: verbs before Hello are rejected.
    [Fact]
    public void VerbsBeforeHello_AreRejected()
    {
        LoggingSession session = NewSession(out _);

        Assert.Throws<InvalidOperationException>(() => session.DefineSchema(LoginSchema()));
        Assert.Throws<InvalidOperationException>(() => session.RecordEvent(ValidEvent()));
        Assert.Throws<InvalidOperationException>(() => session.Goodbye());
    }

    // REQ-0022: Event before a Schema is defined is rejected (a sequencing error, not a data error).
    [Fact]
    public void Event_BeforeSchema_IsRejected()
    {
        LoggingSession session = NewSession(out _);
        session.Hello();

        Assert.Throws<InvalidOperationException>(() => session.RecordEvent(ValidEvent()));
    }

    // REQ-0005 via the session: a valid event is stored and reported valid.
    [Fact]
    public void Event_Valid_IsStored()
    {
        LoggingSession session = NewSession(out InMemoryLogStore store);
        session.Hello();
        session.DefineSchema(LoginSchema());

        ValidationResult result = session.RecordEvent(ValidEvent());

        Assert.True(result.IsValid);
        Assert.Single(store.All());
    }

    // Invalid event DATA returns a result (not a throw) and is not stored.
    [Fact]
    public void Event_Invalid_IsNotStored_AndReturnsResult()
    {
        LoggingSession session = NewSession(out InMemoryLogStore store);
        session.Hello();
        session.DefineSchema(LoginSchema());

        ValidationResult result = session.RecordEvent(InvalidEvent());

        Assert.False(result.IsValid);
        Assert.Empty(store.All());
    }

    // REQ-0024: Goodbye invalidates the token and closes the session.
    [Fact]
    public void Goodbye_InvalidatesToken_AndCloses()
    {
        LoggingSession session = NewSession(out _);
        session.Hello();
        session.DefineSchema(LoginSchema());

        session.Goodbye();

        Assert.Null(session.Token);
        Assert.False(session.IsActive);
    }

    // REQ-0022/0024: any verb after Goodbye is rejected.
    [Fact]
    public void Verbs_AfterGoodbye_AreRejected()
    {
        LoggingSession session = NewSession(out _);
        session.Hello();
        session.Goodbye();

        Assert.Throws<InvalidOperationException>(() => session.RecordEvent(ValidEvent()));
        Assert.Throws<InvalidOperationException>(() => session.Goodbye());
    }

    [Fact]
    public void Session_NullDependencies_AreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new LoggingSession(null!, new InMemoryLogStore()));
        Assert.Throws<ArgumentNullException>(() => new LoggingSession(new StubTokenGenerator(), null!));
    }
}
