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

## Teaching (this is a practice repo)
- **Explain the "why."** When you pick a pattern or design, name it and give a one-paragraph
  rationale. Favor understanding over volume of code.
- **Don't over-engineer to show off.** The simplest design that satisfies the requirement and
  the code policy is the correct one.
