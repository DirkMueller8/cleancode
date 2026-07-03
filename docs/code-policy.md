# Code Policy — Standards & Conventions

The house style for C# in this repo. Enforced partly by [.editorconfig](../.editorconfig);
the rest is on us to uphold. When in doubt, favor clarity over cleverness.

## Language & tooling
- Target `net10.0`, `LangVersion` = `14.0` (or `latest`). Enable `<Nullable>enable</Nullable>`
  and `<ImplicitUsings>enable</ImplicitUsings>` in every project.
- Treat warnings seriously; prefer `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` for
  practice projects so nothing rots.
- Format with `dotnet format` before considering work done.

## Naming & style
- **Use `this.` to access instance members. Do not use `_field` prefixes.** (This is my standing
  preference and is enforced as a warning in `.editorconfig`.)
- `PascalCase` for types, methods, properties, constants, and public members.
- `camelCase` for locals and parameters.
- Interfaces are prefixed with `I` (`IClock`, `IPaymentGateway`).
- One public type per file; file name matches the type.
- Prefer explicit types over `var` when the type isn't obvious from the right-hand side.

## Design principles (the point of this repo)
- **SOLID** — especially:
  - *SRP*: a class has one reason to change. If you're describing it with "and," split it.
  - *DIP*: depend on abstractions. High-level code takes interfaces, not concretions.
- **Dependency Injection** — inject collaborators through the constructor. No `new`-ing up
  dependencies inside a class; no service locators or statics for collaborators.
- **Interfaces first** — define the abstraction where it's *consumed*, keep it small (ISP).
- **Clean Code** — small methods, intention-revealing names, few parameters, no comments that
  restate the code. A comment should explain *why*, not *what*.
- **OOP** — encapsulate state; expose behavior, not data bags. Prefer immutability
  (`readonly`, `init`, records) where it fits.

## Structure
- Solution per topic/exercise; keep production code and tests in separate projects
  (`Foo/` and `Foo.Tests/`).
- Prefer a thin `Program.cs`/composition root that wires up dependencies; keep logic in
  testable classes.

## Testing
- Meaningful units get tests (xUnit unless a requirement says otherwise).
- Test names read as behavior: `Method_State_ExpectsResult`.
- Every requirement's **acceptance criteria** should map to at least one test.

## Errors
- Throw specific exceptions with useful messages; don't swallow exceptions silently.
- Validate arguments at public boundaries (`ArgumentNullException.ThrowIfNull`, guard clauses).
