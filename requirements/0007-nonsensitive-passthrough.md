---
id: REQ-0007
slug: nonsensitive-passthrough
title: Copy nonsensitive fields unchanged into the filtered view
epic: Filtering Engine
status: Draft
priority: Must
scope: now
verification: test
source: ["DSS §2.3", "DSS §5"]
satisfied_by: []
concepts: [Strategy]
stride: []
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an operator, I want nonsensitive fields to appear exactly as logged in the filtered view, so that routine monitoring keeps full access to the data that carries no privacy risk.

## Requirement
Where a field's disposition is `nonsensitive`, the Logger shall copy its value unchanged into the filtered view.

## Worked example
```
Raw:      http: "POST", url: "login.html"
Filtered: http: "POST", url: "login.html"
```

## Acceptance criteria
- [ ] A nonsensitive value is identical before and after filtering.
- [ ] Filtering is a pure function of the input value (no side effects, no context needed).

## Design notes
Introduces the Strategy abstraction `IFieldFilter { FilteredValue Apply(FieldValue value, IPseudonymContext context); }`.
`NonsensitiveFilter` is the first, trivial concrete strategy (ignores the context). Define the interface where it's consumed (the filtered-view assembler), keeping it minimal (ISP).

## Security & traceability
- **Why / rationale:** "Nonsensitive" still means internal-only, but needs no wrapping (DSS §2.3, §5).
- **Source:** DSS §2.3, §5
- **Threat mitigated (STRIDE):** —  ·  **ISO 24772:** —

## Open questions
- None.
