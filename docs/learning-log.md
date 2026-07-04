# Learning Log

A running record of what I practiced and why a design was chosen. One entry per completed
requirement (step 5 of the [workflow](workflow.md)). Newest at the top.

## Template
```
### YYYY-MM-DD — <requirement #> <title>
- **Concept:** <the pattern/principle>
- **Why this design:** <one paragraph>
- **Remember:** <the single most useful takeaway>
```

---

### 2026-07-04 — 0022–0025 Session state machine + in-memory store (Epic E)
- **Concept:** State machine; layering (a new `Logger.Services` project depends on `Logger.Core`);
  matching the failure mechanism to the *kind* of failure.
- **Why this design:** `LoggingSession` models `Hello → Schema → Event* → Goodbye` as New→Active→Closed,
  issuing a token via an injected `ITokenGenerator` (0023) and invalidating it on Goodbye (0024). Events
  are stored in an append-only `InMemoryLogStore` (0025) only when valid. The service stubs live in a
  *separate project* that references the core — the dependency points one way (services → core), which is
  the layering lesson made concrete.
- **Remember:** Two kinds of failure, two mechanisms — deliberately. An **out-of-order verb** (a
  sequencing/contract violation) **throws**; an **invalid event's data** (expected bad input) returns a
  `ValidationResult`. Not all "rejections" are the same: sequencing/programming errors are exceptions,
  routine bad data is a result. Also: append-only is enforced by the *absence* of an update/delete method
  — the illegal operation is unrepresentable, not just discouraged.

### 2026-07-04 — 0019/0020 Query by symbol + inference-attack guard (Epic D)
- **Concept:** Security by design — making the *unsafe* operation unrepresentable, not just discouraged.
- **Why this design:** `FilteredLogQueryEngine` matches on the pseudonym *symbol* (`US1`, hint stripped)
  so operators correlate without seeing raw values (0019). The interesting part is 0020: the engine is
  told which fields are pseudonymized and **refuses a raw/guessed value** on them — you can query
  `[ipaddr = US1]` but not `[ipaddr = 1.1.1.1]`. That closes the inference/oracle attack (guess a value,
  see if it matches, confirm the hidden value) by making the guess un-askable — the same philosophy as
  the deny-by-default unfiltered result that throws.
- **Remember:** A security rule is strongest when the dangerous input can't be expressed. "Reject a
  raw-value query on a pseudonymized field" is better than "return no results" — returning results (even
  empty) is an oracle; refusing to evaluate closes it. I surfaced the "symbol shape" heuristic
  (uppercase letters + digits) as a documented decision, since it defines observable behaviour.

### 2026-07-04 — 0017/0018 Context lifecycle: clear + 24h expiry (Epic C complete)
- **Concept:** Injecting the clock (`IClock`) to make time-dependent behaviour testable; lifecycle management.
- **Why this design:** `PseudonymContextRegistry` keys contexts by (user, log); `Clear` discards one so
  the next access recreates it fresh (0017), and any context idle ≥24h is purged on access (0018). The
  crucial move is **`IClock`**: instead of reading `DateTimeOffset.UtcNow` inside the registry, the time
  source is injected. A `FakeClock` in tests can *advance 25 hours instantly* to prove expiry — a test
  that would otherwise be impossible to write without waiting a day. That's the whole reason `[CCI]`
  (clock issues) warns against reading the system clock directly in logic.
- **Remember:** Any behaviour that depends on "now" should take an `IClock`, not call the system clock.
  It turns an untestable, time-bound rule into a fast, deterministic one. Bonus: the `this.` house rule
  is **compiler-enforced** (`.editorconfig` + `EnforceCodeStyleInBuild`) — the build failed on three
  instance calls I wrote without `this.`, so the convention isn't a suggestion, it's a gate.

### 2026-07-04 — 0050 User-guide generator / DocGen (the "Document" payoff)
- **Concept:** A DI pipeline; interfaces only at substitution boundaries (YAGNI); verify by *running*, not just testing.
- **Why this design:** DocGen is a small pipeline — `IRequirementSource → RequirementParser →
  GuideProjection → IGuideRenderer`, orchestrated by `UserGuideGenerator`. I made interfaces only where
  something is genuinely swappable: the **source** (in-memory stub for tests vs. filesystem) and the
  **renderer** (Markdown today, could change). The parser and projection are pure functions with nothing
  to substitute, so they stayed concrete — interface-izing them would be ceremony, the same speculative
  abstraction the guardrails forbid. The U/I split is by section *heading* (fixed by the template), so
  requirement files needed no retrofitted tags. Frontmatter parsed by hand — no external YAML dep (BCL only).
- **Remember:** Unit tests proved the rules; the real confidence came from **running the tool on the
  actual 17 requirements** and confirming 3 chapters / 14 entries with zero internal-section leaks and
  the two `user_facing:false` specs omitted. "Prove it, don't assert it" means exercising the real thing,
  not just green fixtures. The whole structure we'd maintained for 15 requirements paid off here in one run.

### 2026-07-04 — 0015 Unfiltered view for authorized callers (Epic B complete)
- **Concept:** Making a safety property *structural* rather than a convention; deny-by-default; DIP for auth.
- **Why this design:** `UnfilteredAccessResult` denied → `.Values` throws. That means raw private data
  can't leak even if a caller forgets to check `IsGranted` — the type makes the mistake impossible
  rather than merely discouraged. Authorization is an injected `IUnfilteredAccessPolicy` (the real
  approval/token/audit machinery is Epic H), and denial returns a result rather than throwing because
  a refused request is an expected outcome, not an error.
- **Remember:** The strongest safety controls are the ones the compiler/type-system enforces. "Return
  null on deny" trusts the caller to null-check; "reading Values on a denial throws" doesn't trust
  anyone. Prefer designs where the unsafe path isn't reachable over designs that document not to take it.

### 2026-07-04 — 0014 Assemble the filtered view (the integration moment)
- **Concept:** Composition over abstractions; the payoff of a well-factored engine is a *thin* orchestrator.
- **Why this design:** `FilteredViewAssembler` is deliberately tiny — walk the fields, resolve each
  filter by disposition, apply, collect. It holds **no per-disposition logic** (no `switch`), so it
  composes the six-filter DSS example into one record *and* applies a custom `shout` filter it's never
  heard of, with the same code. That the orchestrator is trivial is the *evidence* the earlier
  abstraction work paid off.
- **Remember:** When the integration layer is boring, the design underneath is right. The whole of Epic
  B converges here into a single passing end-to-end test — nine requirements composing with no glue
  code, no special cases. Also: the assembler *trusts* validation happened earlier (SRP — it composes,
  it doesn't re-validate) but still fails loudly on a broken precondition rather than silently.

### 2026-07-04 — 0013 Pluggable custom filters (the Open/Closed payoff)
- **Concept:** Open/Closed Principle, proven; Interface Segregation; retiring a fake once the real thing exists.
- **Why this design:** The whole epic was sequenced for this moment. With four real filters already
  behind `IFieldFilter`, the `FilterRegistry` proves a *fifth* (a test-only `ShoutFilter`) needs only a
  new class + one `Register` call — **zero edits** to any existing filter or the assembler. That's OCP
  demonstrated against real variety, not a toy. `IFilterRegistry` grew to add `Resolve`, but `Register`
  stayed off the interface (ISP): query-side clients (`Schema`, the assembler) can't mutate the registry.
- **Remember:** Sequencing matters — building the OCP seam *after* four concrete strategies made the
  "open for extension" claim credible and testable. Also: once the real `FilterRegistry` existed, the
  `FakeFilterRegistry` stub from REQ-0004 was **dead weight**, so I deleted it and wired tests to the
  real registry — and every prior test stayed green, which is what makes a cleanup safe to do.

### 2026-07-04 — 0011/0012 Minute + country filters
- **Concept:** Two more Strategy implementations; DRY extraction; a sign-safe floor; behaviour-preserving refactor.
- **Why this design:** `MinuteFilter` (floor epoch to the minute) and `CountryFilter` (IP → `US1(v4)` via
  an injected `IGeoLookup`) are the third and fourth built-in filters — deliberately built *before* the
  OCP requirement (0013) so the pluggable-filter claim is proven against real variety. Per 0012's
  resolution I extracted a shared `Pseudonym.Format` and refactored `PrivateFilter` onto it (DRY), then
  confirmed PrivateFilter's tests stayed green — a behaviour-preserving refactor.
- **Remember:** The minute floor used `((e % 60) + 60) % 60`, not `e - (e % 60)`, because `%` rounds
  toward zero in C# — a bare version silently mis-floors negative epochs. Small, real, exactly the kind
  of `[XZH]` sign bug that boundary thinking catches. And `CountryFilter` reuses the *pseudonym context*
  seam with the country as the prefix — the same machinery serving a very different-looking filter,
  which is the sign the abstraction is right.

### 2026-07-04 — 0008/0009/0010/0016 Private wrapping + pseudonym context (cluster)
- **Concept:** Building a tightly-coupled cluster together; DIP seams; the parameter object; verifying
  a *structural* property behaviourally.
- **Why this design:** A pseudonym is meaningless without a context to be stable within, so these four
  were designed as one. The context (`PseudonymContext`) owns stable per-prefix numbering (0009), keyed
  by a **salted digest** via an injected `IHasher` so the raw value is never stored (0010), and is one
  instance per (user, log) view for isolation (0016). `PrivateFilter` composes a prefix (`IPrefixPolicy`
  seam) + the context's number + a length hint into `USER1(3)` (0008). Three DIP seams (`IHasher`,
  `IPseudonymContext`, `IPrefixPolicy`) mean every collaborator is swappable and testable.
- **The interface grew, as promised.** `IFieldFilter.Apply(string)` became `Apply(FilterInput)` — a
  parameter object bundling `{ FieldName, Value, Pseudonyms }`, so future per-call data won't churn
  every filter. Updating `NonsensitiveFilter` was the trivial 2-file change I predicted at 0007.
- **Remember:** "The context stores no raw value" is a structural claim with no clean public accessor.
  Rather than expose internals just to assert on them, I proved it **behaviourally**: inject a colliding
  hasher so two different values collapse to one number — only possible if the *digest* is the key.
  Prefer a black-box proof over widening visibility for a test.

### 2026-07-04 — 0007 Copy nonsensitive fields unchanged (Epic B begins)
- **Concept:** Strategy pattern; YAGNI applied to *interface design*.
- **Why this design:** `IFieldFilter` is the Strategy seam every filter will implement; `NonsensitiveFilter`
  is the trivial first one (a pure passthrough). I deliberately defined the interface as
  `string Apply(string value)` — *not* the `Apply(value, context)` the design note sketched — because
  nothing in 0007 needs a context. Adding it now would be speculative code (guardrail) and would couple
  the passthrough filter to state it never uses.
- **Remember:** Interfaces are best grown, not divined. With one implementer, widening `Apply` at
  REQ-0008 (when the pseudonym context actually appears) is a 2-file change. Watching the interface
  evolve *is* the lesson — resist the urge to design it "complete" before a requirement demands it.

### 2026-07-04 — 0006 Enforce request and field size limits (Epic A complete)
- **Concept:** Reuse over reinvention; boundary-value testing; one source of truth for constants.
- **Why this design:** The oversized-input check returns the *same* `ValidationResult` type built for
  REQ-0005 rather than a new result type — same failure semantics, so reuse it. Limits live in one
  `Limits` class (change them in one place). Two separate checks because they act on two different
  inputs: the raw request text (1M limit, service-layer input) vs. the parsed event's field values
  (10k limit). Kept out of `LogEvent`'s constructor so construction stays total and validation stays a
  separate concern (SRP + the throw-vs-result rule from 0005).
- **Remember:** Boundary-value tests (N and N+1) are where off-by-one and `>` vs `>=` bugs live —
  cheap to write, high signal. And I surfaced the "characters = UTF-16 code units" caveat rather than
  let `string.Length` silently define an observable behavior.

### 2026-07-04 — 0005 Accept an event only if it matches its schema
- **Concept:** Exceptions vs. result objects — matching the mechanism to the *expectedness* of failure.
- **Why this design:** Bad *schemas* throw (rare, programmer error, caught at build time). Bad *events*
  return a `ValidationResult` — they're an expected, high-volume runtime outcome from untrusted input,
  so throwing would be both slow and semantically wrong (an invalid event isn't "exceptional"). The
  result also forces the caller to check `IsValid` rather than silently proceed (ISO 24772 `[OYB]`).
  The validator collects *all* violations, and is constructed with its `Schema` (injectable, DIP).
- **Remember:** "Throw or return a result?" is answered by one question — *is this failure expected in
  normal operation?* If yes, return a result; reserve exceptions for genuine programmer/contract errors.
  Also surfaced two silent decisions: event values are text (DSS canonical form), and `Time` = Unix
  epoch seconds — both documented + tested rather than left implicit.

### 2026-07-04 — 0003 Require a timestamp and one identifying field
- **Concept:** Choosing the right *level* to enforce an invariant; surfacing silent decisions (again).
- **Why this design:** Put the rule at `Schema` construction, not in `LogType`. The requirement says
  "reject the *schema*," it needs no registry, and — the deciding factor — keeping `LogType` a pure
  structural type meant none of REQ-0002's LogType-only tests broke. Level of enforcement is a real
  design lever: too low and you couple unrelated concerns; too high and illegal states slip through.
- **Remember:** The spec was silent on two observable things, so I decided *and documented* both rather
  than defaulting quietly: (1) "the timestamp" is identified by **type** (`Time`), not the name
  `"timestamp"`; (2) two `Time` fields are accepted. Each got a line in the requirement and a test.
  That's the medical-device reflex in miniature — an assumption you can't avoid becomes a written,
  verified decision, not a buried one.

### 2026-07-03 — 0004 Every field must declare a disposition
- **Concept:** Dependency Inversion + Interface Segregation; "make illegal states unrepresentable."
- **Why this design:** 0004 needed to validate disposition names at build time, but the real filter
  registry is REQ-0013 (later). Instead of pulling 0013 forward, I introduced a *minimal* abstraction
  `IFilterRegistry` with one method, `IsRegistered(name)` — the only thing 0004 consumes (ISP). `Schema`
  depends on the abstraction (DIP) and is tested with a `FakeFilterRegistry` stub, so no real filters
  are needed yet. Making `Disposition` a required, non-null constructor arg on `FieldDefinition` means
  "every field must declare a disposition" is enforced by the type system, not a runtime check I could
  forget.
- **Remember:** When requirement A depends on requirement B that isn't built yet, don't build B early —
  extract the *smallest* interface A actually needs and stub it in tests. The seam is real; the
  implementation can wait. Also: I deliberately did **not** special-case `nonsensitive`/`private` — they
  validate through the registry like any name, so there's one rule, not two (matches REQ-0013).

### 2026-07-03 — 0002 Define a log schema of typed fields
- **Concept:** Immutable value objects + "validate in the constructor" (make illegal states unrepresentable).
- **Why this design:** `Schema` → `LogType` → `FieldDefinition` each validate their invariants in the
  constructor (no duplicate log-type/field names, non-blank names), so you can never hold a malformed
  schema object. That's simpler and safer than a separate validator you might forget to call. I chose
  a constructor over the "builder" the spec hinted at, because there was no multi-step construction to
  justify a builder yet — I'll add one only if REQ-0004's registry validation forces it (YAGNI).
- **Remember:** A closed `enum` (`FieldType`) makes "exactly these four types" a compile-time fact —
  but enums can still hold undefined values via a raw cast, so guard with `Enum.IsDefined` the day we
  parse a type from outside (ISO 24772 `[CCB]`). First green build set up `Directory.Build.props` with
  `TreatWarningsAsErrors` so quality is enforced by the compiler, not willpower.

<!-- Older example kept for reference:

### 2026-07-03 — 0001 FizzBuzz via a rule set
- **Concept:** Open/Closed Principle + Strategy.
- **Why this design:** Modeling each rule as an IFizzBuzzRule lets new rules be added as new
  classes, so the converter never changes — closed for modification, open for extension.
- **Remember:** When a requirement says "without editing existing code," reach for an abstraction
  the caller composes, not an if/else the caller edits.
-->
