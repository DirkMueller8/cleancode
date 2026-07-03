# Guardrails — Hard Rules

These are non-negotiable. They win over the requirement, the code policy, and convenience.
If following a requirement would break one of these, **stop and ask me**.

## Process
- **No feature code without a requirement.** If I ask for a feature and there's no matching
  file in [requirements/](requirements/), either draft one first or ask me to.
- **Plan before building.** For anything beyond a one-liner, show a short plan and wait for my
  "go" before writing code.
- **One requirement at a time.** Don't smuggle in unrelated changes. If you spot something else
  worth doing, note it — don't do it.
- **Ask, don't assume.** If a requirement is ambiguous, ask a specific question rather than
  guessing and building the wrong thing.

## Code
- **No unrequested dependencies.** Don't add a NuGet package without asking. Prefer the BCL.
- **No dead or speculative code.** Build what the requirement asks for, not "might need it later."
- **No suppressing warnings** (`#pragma warning disable`, `<NoWarn>`) to make something pass.
  Fix the cause or tell me why the warning is wrong.
- **No secrets in the repo.** No connection strings, keys, or tokens committed. Use user-secrets
  or environment variables.
- **Don't weaken tests to make them pass.** A red test means the code is wrong until proven otherwise.

## Honesty
- **Report reality.** If the build fails, tests fail, or a step was skipped, say so plainly with
  the actual output. Never claim "done" for something unverified.
- **Flag when I'm wrong.** If a requirement asks for something that conflicts with SOLID, Clean
  Code, or a guardrail, tell me — this is a learning repo, so the disagreement is the point.

## Language safety (per ISO/IEC 24772-1)
The Tier-1 vulnerabilities that plain managed C# can actually hit. Full analysis and the other
tiers are in [iso-24772-applicability.md](iso-24772-applicability.md); cite the `[CODE]` when a
change addresses one. These are checkable rules, not just advice:

- **Null safety `[XYH]`** — enable nullable reference types (`<Nullable>enable</Nullable>`) and
  treat null warnings as errors. Guard public inputs with `ArgumentNullException.ThrowIfNull`.
- **Never swallow errors `[OYB]`** — no empty `catch`; don't ignore a `TryX` result or return code.
  Catch narrowly and either handle or rethrow.
- **Overflow is silent `[FIF]`/`[FLC]`** — integer math wraps in the default `unchecked` context;
  use `checked` where correctness depends on it, and `TryParse` over narrowing casts.
- **Floating-point `[PLF]`** — never `==` on `double`/`float`; use `decimal` for money.
- **Enums accept any value `[CCB]`** — validate external ints with `Enum.IsDefined`; give `switch`
  a throwing `default`.
- **Exhaustive control flow `[CLL]`/`[EOJ]`** — brace every block (enforced in `.editorconfig`);
  handle every `switch` case.
- **Concurrency `[CGX]`/`[CGM]`** — no shared mutable state without a lock or a concurrent/immutable
  type; consistent lock ordering; no `async void` (except event handlers); use `CancellationToken`.
- **OOP contracts `[BLP]`/`[PPH]`** — subtypes honor base pre/post-conditions; never call a
  `virtual` member from a constructor.
- **Lifetime `[XYL]`** — unsubscribe events, dispose `IDisposable` (prefer `using`); GC does not
  cover these leaks.
- **`unsafe`/interop is opt-in only `[MXB]`/`[SKL]`** — no `unsafe`, P/Invoke, or `#nullable disable`
  without a requirement that calls for it; then re-read the Tier-2 rows first.

## Teaching (this is a practice repo)
- **Explain the "why."** When you pick a pattern or design, name it and give a one-paragraph
  rationale. Favor understanding over volume of code.
- **Don't over-engineer to show off.** The simplest design that satisfies the requirement and
  the code policy is the correct one.
