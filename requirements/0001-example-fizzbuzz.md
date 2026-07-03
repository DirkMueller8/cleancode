# 0001 — FizzBuzz via an extensible rule set

- **Status:** Draft
- **Created:** 2026-07-03
- **Concept(s) practiced:** Open/Closed Principle, Dependency Inversion, Strategy pattern

> This is a worked example showing how a requirement should look. Copy
> [TEMPLATE.md](TEMPLATE.md) for real work; leave this one as a reference.

## Intent
Practice designing for extension: produce FizzBuzz output, but structure it so new rules
(e.g. "Bazz" on multiples of 7) can be added without editing existing classes.

## Scope
**In scope**
- Converting an integer to its FizzBuzz string.
- A rule abstraction that new rules plug into.
- A console entry point that prints 1..N.

**Out of scope**
- Configuration files, user input parsing beyond a single `N`.
- Any UI beyond console output.

## Behavior
- For each number `1..N`: multiples of 3 → `Fizz`, of 5 → `Buzz`, of both → `FizzBuzz`,
  otherwise the number itself.
- Rules are applied in order and their outputs concatenated; if no rule matches, print the number.

## Acceptance criteria
- [ ] `3 -> "Fizz"`, `5 -> "Buzz"`, `15 -> "FizzBuzz"`, `7 -> "7"`.
- [ ] Adding a new rule requires creating a new class only — no edits to existing rule classes
      or the converter (Open/Closed).
- [ ] The converter depends on a rule abstraction (`IFizzBuzzRule`), not concrete rules (DIP).
- [ ] Rules are injected into the converter via its constructor (DI).
- [ ] Tests cover each criterion above.

## Design notes
- `IFizzBuzzRule { string? Apply(int n); }` — returns its token or `null` if it doesn't match.
- `FizzBuzzConverter(IEnumerable<IFizzBuzzRule> rules)` — composes matches, falls back to `n`.
- Concrete rules: `FizzRule`, `BuzzRule`. Composition root builds the list.

## Open questions
- None for the example.
