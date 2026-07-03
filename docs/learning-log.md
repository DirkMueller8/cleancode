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
