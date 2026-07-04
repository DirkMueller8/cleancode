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
