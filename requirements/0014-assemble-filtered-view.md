---
id: REQ-0014
slug: assemble-filtered-view
title: Assemble a full filtered view of an event
epic: Filtering Engine
status: Done
priority: Must
scope: now
verification: test
source: ["DSS §2.3", "DSS §5"]
satisfied_by: ["tests/Logger.Core.Tests/FilteredViewAssemblerTests.cs"]
concepts: [Composition, DIP, SRP, OCP]
stride: [InformationDisclosure]
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an operator, I want to see a whole event in filtered form, so that each field is shown according to
its declared disposition in a single coherent record.

## Requirement
When an event is filtered, the Logger shall produce a filtered view in which each field has been
transformed by the filter registered for that field's disposition.

## Worked example
```
Schema "login": timestamp:minute, ipaddr:country, http:0, url:0, user:private, password:private
Raw:      2022/10/19 08:09:10  66.77.88.99  POST  login.html  {user: "SAM", password: ">1<}2{]3[\4/"}
Filtered: 2022/10/19 08:09     US1(v4)      POST  login.html  {user: USER1(3), password: PW1(12)}
```

## Acceptance criteria
- [x] Every field in the output is the result of its disposition's filter.
- [x] Field order and names are preserved.
- [x] Given the schema above, the raw example produces exactly the filtered example.
- [x] The assembler depends only on `IFieldFilter` / the registry — it contains no per-disposition logic.

## Design notes
`FilteredViewAssembler(FilterRegistry)` iterating fields, resolving each filter by disposition, passing
the shared `IPseudonymContext`. Pure composition over REQ-0007–0013; SRP (assembly only) and DIP
(depends on abstractions). This is the integration point the earlier requirements build toward.

**As built (2026-07-04):**
- Iterates `logType.Fields` (schema declared order → `FilteredView` order); resolves each disposition
  via `IFilterRegistry.Resolve`; shares one `IPseudonymContext` across all fields so pseudonyms stay
  consistent within the event.
- *No per-disposition logic:* proven by a test where a custom `shout` filter (unknown to the assembler)
  is applied purely by resolution — the OCP property holds at the composition level too.
- *Precondition:* the event is assumed schema-validated (REQ-0005); a missing declared field is a
  contract violation and is rejected with a clear message rather than silently skipped.
- The headline test is the DSS §2.3 worked example, verified end to end.

## Security & traceability
- **Why / rationale:** The filtered view is the artifact routine users see (DSS §2.3); it must apply
  every field's disposition consistently.
- **Source:** DSS §2.3, §5
- **Threat mitigated (STRIDE):** InformationDisclosure  ·  **ISO 24772:** —

## Open questions
- None.
