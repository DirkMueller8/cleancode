# Workflow — The Spec-Driven Development Harness

This is the loop we run for every piece of feature work. It exists so that code follows
intent, and so that I practice thinking in requirements before implementation.

```
Specify  ->  Plan  ->  Build  ->  Verify  ->  Reflect
   ^                                              |
   +----------------  (next requirement)  --------+
```

## 1. Specify
Capture the goal as a requirement file in [requirements/](requirements/).

- Copy [requirements/TEMPLATE.md](../requirements/TEMPLATE.md) to
  `requirements/NNNN-short-title.md` (next free number).
- Fill in the intent, scope, and **acceptance criteria**. Criteria must be checkable
  (a test can pass or fail against each one).
- **Claude may help:** if I give a rough idea, draft the requirement and let me refine it.
  Ask questions where the intent is unclear rather than inventing details.
- Set `Status: Draft`.

## 2. Plan
Before writing code, Claude produces a **short** plan:
- Which types/interfaces will exist and how they collaborate.
- Which SOLID / design concept this exercises (name it — this is a learning repo).
- The test list that will prove the acceptance criteria.

I approve or adjust. Move requirement to `Status: In progress`.

## 3. Build
Implement per [code-policy.md](code-policy.md) and within [guardrails.md](guardrails.md).
- Composition root wires dependencies; logic lives in injectable classes.
- Write tests alongside the code, mapped to acceptance criteria.
- No scope creep, no unrequested packages.

## 4. Verify
Prove it works — don't assert it.
- `dotnet build` clean (warnings treated as errors where enabled).
- `dotnet test` green.
- Walk each acceptance criterion and confirm a test or a demonstrated run covers it.
- Report the actual output. If something failed or was skipped, say so.

Move requirement to `Status: Done` only when every criterion is met.

## 5. Reflect
Because this repo is for learning:
- Add a dated entry to [learning-log.md](learning-log.md): the concept, why the design was
  chosen, and one thing to remember.
- If it's a durable preference or lesson about *how I want to work*, tell Claude to remember it.

## Roles
- **Me:** owns intent, priorities, and approval gates (plan sign-off, "Done").
- **Claude:** drafts specs on request, plans, implements, verifies, teaches the "why," and
  stops to ask when a requirement is ambiguous or collides with a guardrail.
