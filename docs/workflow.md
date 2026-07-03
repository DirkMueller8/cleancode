# Workflow — The Spec-Driven Development Harness

This is the loop we run for every piece of feature work. It exists so that code follows
intent, and so that I practice thinking in requirements before implementation.

```
Specify  ->  Plan  ->  Build  ->  Verify  ->  Reflect
   ^                                              |
   +----------------  (next requirement)  --------+

               ... then Document (step 6): generate the user guide from the requirements.
```

## 1. Specify
Capture the goal as a requirement file in [requirements/](requirements/). Method and quality bar:
[requirements-engineering.md](requirements-engineering.md).

- **One file = one singular requirement.** Copy [requirements/TEMPLATE.md](../requirements/TEMPLATE.md)
  to `requirements/NNNN-short-title.md` (next free number). Epics are tracked in
  [requirements/README.md](../requirements/README.md), not as files.
- Fill in the frontmatter (priority, scope, traceability, security tags) and the body. Write the
  **Requirement** in EARS where natural, and **acceptance criteria** that are checkable (one test can
  pass/fail each).
- Tag body sections **(U)** user-facing or **(I)** internal — the doc generator (step 6) depends on it.
- **Claude may help:** if I give a rough idea, draft the requirement and let me refine it.
  Ask questions where the intent is unclear rather than inventing details.
- Set `status: Draft`.

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

## 6. Document (extract the user guide)
User documentation is **not written by hand** — it's projected from the requirements. Each
requirement's **(U)**-tagged sections, for `user_facing: true` items, compile into the user guide,
grouped by `doc_chapter`. The `tools/DocGen` generator (a planned requirement) performs this; until it
exists, the guide is *latent* in the corpus — disciplined (U)/(I) tagging is what makes it extractable
later. Internal sections (rationale, design, tests, traceability) are never emitted.

## Roles
- **Me:** owns intent, priorities, and approval gates (plan sign-off, "Done").
- **Claude:** drafts specs on request, plans, implements, verifies, teaches the "why," and
  stops to ask when a requirement is ambiguous or collides with a guardrail.
