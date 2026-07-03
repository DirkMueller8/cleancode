---
id: REQ-0013
slug: pluggable-custom-filters
title: Support pluggable custom filters without modifying existing code
epic: Filtering Engine
status: Draft
priority: Must
scope: now
verification: test
source: ["DSS §5"]
satisfied_by: []
concepts: [OCP, Strategy, DIP]
stride: []
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an application author, I want to add my own field filter for a data type Logger doesn't ship, so
that I can classify custom data without changing Logger's code.

## Requirement
Where an application registers a custom filter under a disposition name, the Logger shall apply that
filter to fields declaring that disposition, without any modification to existing filter classes or
the view assembler.

## Worked example
```
Register:  "postcode" -> PostcodeFilter   (a new class the app supplies)
Schema:    zip:String disposition=postcode
Filtered:  zip -> ZIP1(5)                  (behavior defined by PostcodeFilter)
```

## Acceptance criteria
- [ ] A newly registered filter is applied to fields with its disposition name.
- [ ] Adding a filter requires only a new `IFieldFilter` class plus a registration call — no edits to
      existing filters or the assembler (verified by the design; demonstrated by a test-only filter).
- [ ] Registering a duplicate disposition name is rejected (or explicitly overrides — decide below).
- [ ] An unknown disposition name (never registered) is rejected at schema validation (ties REQ-0004).

## Design notes
**This is the Open/Closed Principle headline of the epic** — deliberately sequenced after four
built-in strategies (nonsensitive, private, minute, country) already exercise the seam. A
`FilterRegistry` maps disposition name → `IFieldFilter`; the assembler resolves filters through it and
never switches on type. Built-ins are registered the same way custom ones are (no special-casing).

## Security & traceability
- **Why / rationale:** DSS §5: "Filters should be defined by pluggable components and easily extended to support custom data types."
- **Source:** DSS §5
- **Threat mitigated (STRIDE):** —  ·  **ISO 24772:** —

## Open questions
- ~~Duplicate registration: reject or last-wins?~~ **Resolved (2026-07-03): reject.** Registering a
  second filter under an existing disposition name is an error, to avoid a silently overridden filter.
  (Already reflected in the acceptance criteria.)
