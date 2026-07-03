# cleancode

A personal practice repo for C# / .NET 10 (C# 14), built around **spec-driven development**
with Claude. The point is as much the *method* as the code: write a requirement, plan against
it, build, verify, and record what I learned.

## Start here
- **[CLAUDE.md](CLAUDE.md)** — the operating manual and index. Read this first.

## The rules
1. **[docs/guardrails.md](docs/guardrails.md)** — hard do/don't (these win).
2. **[docs/code-policy.md](docs/code-policy.md)** — coding standards & conventions.
3. **[docs/workflow.md](docs/workflow.md)** — the Specify → Plan → Build → Verify → Reflect loop.

## Where things live
- **[requirements/](requirements/)** — one spec per feature; start from
  [TEMPLATE.md](requirements/TEMPLATE.md).
- **[docs/learning-log.md](docs/learning-log.md)** — what I practiced and why.
- **[.editorconfig](.editorconfig)** — enforces the `this.` convention and formatting.

## Environment
- .NET 10 (`10.0.301`) default; .NET 9 available as fallback. Windows 11 / PowerShell.

## The loop, in one line
> Write a requirement → Claude plans → we build to policy → verify every acceptance criterion → log the lesson.
