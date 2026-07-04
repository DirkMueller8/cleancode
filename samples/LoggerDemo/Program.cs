using Logger.Core;
using Logger.Services;

Console.WriteLine("=== Logger demo — privacy-aware logging in action ===\n");

// 1) Composition root: register the filters (built-ins register like any custom filter).
var registry = new FilterRegistry();
registry.Register(Disposition.NonsensitiveName, new NonsensitiveFilter());
registry.Register(Disposition.PrivateName, new PrivateFilter(new DefaultPrefixPolicy()));
registry.Register("minute", new MinuteFilter());
registry.Register("country", new CountryFilter(new DemoGeoLookup()));

// 2) Declare the schema: which fields exist and how each is handled.
var login = new LogType("login", new[]
{
    new FieldDefinition("timestamp", FieldType.Time, new Disposition("minute")),
    new FieldDefinition("ipaddr", FieldType.IpAddress, new Disposition("country")),
    new FieldDefinition("user", FieldType.String, Disposition.Private),
    new FieldDefinition("password", FieldType.String, Disposition.Private),
});
var schema = new Schema(new[] { login }, registry);
Console.WriteLine("Schema 'login': timestamp=minute  ipaddr=country  user=private  password=private\n");

// 3) A session: Hello -> Schema -> Event* -> Goodbye.
var store = new InMemoryLogStore();
var session = new LoggingSession(new SecureTokenGenerator(), store);
string token = session.Hello();
Console.WriteLine($"Hello   -> session active; token {token[..8]}… ({token.Length * 4} bits)");
session.DefineSchema(schema);

var events = new[]
{
    NewEvent("1729324150", "66.77.88.99", "SAM", "hunter2pass!"),
    NewEvent("1729324211", "66.77.88.99", "BOB", "pw"),
    NewEvent("1729324300", "10.0.0.7", "SAM", "xyz"),
};
foreach (LogEvent e in events)
{
    ValidationResult result = session.RecordEvent(e);
    Console.WriteLine($"Event   -> {(result.IsValid ? "stored " : "REJECT ")}: {Raw(e)}");
}

Console.WriteLine($"\n{store.All().Count} events stored (append-only).\n");

// 4) Filtered view — one shared per-viewer context, so pseudonyms are consistent.
IPseudonymContext context = new PseudonymContextRegistry(new Sha256Hasher(), new SystemClock())
    .GetContext("investigator", "login");
var assembler = new FilteredViewAssembler(registry);
var filtered = events.Select(e => assembler.Assemble(login, e, context)).ToList();

Console.WriteLine("Filtered view (routine access — raw values are never shown):");
foreach (FilteredView view in filtered)
{
    Console.WriteLine("   " + Show(view, "timestamp", "ipaddr", "user", "password"));
}

// 5) Correlation without disclosure — query by the symbols the operator can see.
Console.WriteLine("\nCorrelation without disclosure:");
var engine = new FilteredLogQueryEngine(new[] { "ipaddr", "user", "password" });
Console.WriteLine($"   [ipaddr = US1] -> {engine.Search(filtered, Cond("ipaddr", "US1")).Count} events (same hidden IP)");
Console.WriteLine($"   [user = USER1] -> {engine.Search(filtered, Cond("user", "USER1")).Count} events (same hidden user, across IPs)");

// 6) Inference guard — a raw guess on a hidden field is refused, not evaluated.
Console.Write("   [ipaddr = 66.77.88.99] (a raw guess) -> ");
try
{
    engine.Search(filtered, Cond("ipaddr", "66.77.88.99"));
    Console.WriteLine("matched?!");
}
catch (ArgumentException)
{
    Console.WriteLine("REJECTED (can't confirm a hidden value by guessing)");
}

// 7) Graduated access — the unfiltered (raw) view only for the authorized.
Console.WriteLine("\nUnfiltered (raw) access:");
var provider = new UnfilteredViewProvider(new AllowList("investigator"));
UnfilteredAccessResult granted = provider.Reveal(events[0], new AccessRequest("investigator", "login"));
Console.WriteLine($"   investigator -> GRANTED: user={granted.Values["user"]}  password={granted.Values["password"]}");
UnfilteredAccessResult denied = provider.Reveal(events[0], new AccessRequest("guest", "login"));
Console.WriteLine($"   guest        -> {(denied.IsGranted ? "granted" : "DENIED (no raw values returned)")}");

// 8) End the session — the token is invalidated.
session.Goodbye();
Console.WriteLine($"\nGoodbye -> token invalidated; session active = {session.IsActive}.");

static LogEvent NewEvent(string timestamp, string ip, string user, string password) =>
    new("login", new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["timestamp"] = timestamp,
        ["ipaddr"] = ip,
        ["user"] = user,
        ["password"] = password,
    });

static string Raw(LogEvent e) =>
    string.Join("  ", e.Values.Select(kv => $"{kv.Key}={kv.Value}"));

static string Show(FilteredView view, params string[] fields) =>
    string.Join("  ", fields.Select(f =>
    {
        view.TryGetValue(f, out string? value);
        return $"{f}={value}";
    }));

static IReadOnlyList<QueryCondition> Cond(string field, string value) =>
    new[] { new QueryCondition(field, value) };

/// <summary>Demo-only geo lookup: every address resolves to the US (real geo is out of scope).</summary>
internal sealed class DemoGeoLookup : IGeoLookup
{
    public string CountryOf(string ipAddress) => "US";
}

/// <summary>Demo-only access policy: grants unfiltered access only to a fixed allow-list of users.</summary>
internal sealed class AllowList : IUnfilteredAccessPolicy
{
    private readonly HashSet<string> allowed;

    public AllowList(params string[] users) => this.allowed = new HashSet<string>(users, StringComparer.Ordinal);

    public bool IsAllowed(AccessRequest request) => this.allowed.Contains(request.User);
}
