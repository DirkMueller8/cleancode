---
id: REQ-0050
slug: user-guide-generator
title: Generate the user guide from the requirements
epic: Tooling
status: Done
priority: Should
scope: now
verification: test
source: ["docs/workflow.md §6", "docs/requirements-engineering.md §8"]
satisfied_by: ["tests/DocGen.Tests/UserGuideGeneratorTests.cs"]
concepts: [DIP, SRP, Pipeline, YAGNI]
stride: []
iso24772: []
user_facing: false
doc_chapter: "Tooling"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As the repo owner, I want the user guide generated from the requirement files, so that user
documentation never drifts from the specs and I don't hand-write it.

## Requirement
The generator shall read the requirement files, select those with `user_facing: true` (excluding
`scope: out-of-scope-infra`), and emit a single user guide containing only their user-facing sections
(`Summary`, `Requirement`, `Worked example`), grouped by `doc_chapter` and ordered by `id`.

## Worked example
```
requirements/0002 (user_facing, chapter "Defining what you log")  ->  ## Defining what you log
                                                                      ### Define a log schema of typed fields
                                                                      <Summary + Requirement + Worked example>
requirements/0010 (user_facing: false)                            ->  (omitted)
```

## Acceptance criteria
- [x] Only requirements with `user_facing: true` appear in the guide.
- [x] Requirements with `scope: out-of-scope-infra` are excluded.
- [x] For an included requirement, only the U sections appear; internal sections (Acceptance criteria,
      Design notes, Security & traceability, Open questions) do **not** appear.
- [x] Requirements are grouped by `doc_chapter`; chapters are ordered by their lowest `id`.
- [x] Non-requirement files (TEMPLATE, README, glossary, the legacy 0001) are ignored.
- [x] Generation is deterministic — the same inputs produce the same output.

<!-- Verified end-to-end (2026-07-04): `dotnet run --project tools/DocGen` against the real
requirements/ produced docs/user/user-guide.md with 3 chapters and 14 user-facing requirements;
internal sections and the two user_facing:false requirements (0010, 0050) were correctly omitted. -->


## Design notes
Pipeline `IRequirementSource → RequirementParser → GuideProjection → IGuideRenderer`, orchestrated by
`UserGuideGenerator.Generate()` which returns the guide text; the console `Program` writes it to
`docs/user/user-guide.md`. **Interfaces only at substitution boundaries** (source for test injection,
renderer for output format) — the parser and projection are pure and stay concrete (YAGNI). The
U/I split is by **section heading** (per the template), so requirement files need no inline tags.
Frontmatter is parsed directly (`key: value`) — no external YAML dependency (BCL only).

## Security & traceability
- **Why / rationale:** Fulfils step 6 (Document) of the workflow — docs projected from specs, not
  written by hand. See [workflow.md](../docs/workflow.md) and [requirements-engineering.md](../docs/requirements-engineering.md).
- **Source:** docs/workflow.md §6
- **Threat mitigated (STRIDE):** —  ·  **ISO 24772:** —

## Open questions
- None.
