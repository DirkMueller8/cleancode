# CLAUDE.md — Operating Manual

This file is the entry point for working with Claude in this repository. Read it first,
then follow the links to the detailed policy documents. Keep this file short; put detail
in the linked docs.

## Purpose
- Practice and sharpen my core C# skills.
- Learn new concepts from **.NET 10 / C# 14** onward.
- Do it through **spec-driven development**: I (optionally with Claude's help) write a
  requirement, and we build against it. This repo is as much about the *method* as the code.

## How we work here (the harness)
Every non-trivial change follows the loop in [docs/workflow.md](docs/workflow.md):

1. **Specify** — capture the goal as a requirement in [requirements/](requirements/) using
   [TEMPLATE.md](requirements/TEMPLATE.md). Claude may help draft it.
2. **Plan** — Claude proposes a short implementation plan and waits for my go-ahead.
3. **Build** — Claude implements per [docs/code-policy.md](docs/code-policy.md), honoring the
   hard rules in [docs/guardrails.md](docs/guardrails.md).
4. **Verify** — build, test, and check every acceptance criterion in the requirement.
5. **Reflect** — record the concept I practiced in [docs/learning-log.md](docs/learning-log.md).

Do not skip step 1 for feature work. Trivial fixes (typos, formatting) may skip straight to Build.

## The rules, in priority order
1. [docs/guardrails.md](docs/guardrails.md) — hard do/don't. These win over everything else.
2. [docs/code-policy.md](docs/code-policy.md) — coding standards and conventions.
3. [docs/workflow.md](docs/workflow.md) — the process to follow.

If a requirement conflicts with a guardrail, stop and tell me — do not silently resolve it.

## Environment
- **Default SDK:** .NET 10 (`10.0.301`) at `C:\Program Files\dotnet`. Target `net10.0`, `LangVersion` = `14.0` (or `latest`).
- **Fallback:** .NET 9 (`9.0.315`) available on the same machine.
- **OS / shell:** Windows 11, PowerShell (a Bash tool is also available for POSIX scripts).
- Verify with `dotnet --version` before assuming a toolchain.

## Topics I'm reinforcing
Interfaces · Dependency Injection / Inversion of Control · SOLID · Clean Code · OOP.
When you implement something that exercises one of these, name it explicitly so I notice it.

## Learning method
- Memorize the design pattern being used, then have Claude explain the solution.
- Prefer a short explanation of *why* a design was chosen over a wall of code.
