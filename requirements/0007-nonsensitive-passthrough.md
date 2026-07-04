---
id: REQ-0007
slug: nonsensitive-passthrough
title: Copy nonsensitive fields unchanged into the filtered view
epic: Filtering Engine
status: Done
priority: Must
scope: now
verification: test
source: ["DSS §2.3", "DSS §5"]
satisfied_by: ["tests/Logger.Core.Tests/NonsensitiveFilterTests.cs"]
concepts: [Strategy, YAGNI]
stride: []
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-04
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
- [x] A nonsensitive value is identical before and after filtering.
- [x] Filtering is a pure function of the input value (no side effects, no context needed).

## Design notes
Introduces the Strategy abstraction `IFieldFilter`. `NonsensitiveFilter` is the first, trivial concrete
strategy. The filtered-view assembler (REQ-0014) resolves filters by disposition name via the registry.

**As built (2026-07-04) — interface kept minimal (YAGNI):** the original sketch was
`Apply(value, context)`, but a nonsensitive passthrough needs no context, so the interface is defined
as just `string Apply(string value)`. It will **grow** to include a pseudonym context when REQ-0008
requires it. Rationale: honor the guardrail against speculative code, and model incremental interface
design — an abstraction earns its parameters as requirements reveal the need. With one implementer
today, evolving the signature at REQ-0008 is cheap.

## Security & traceability
- **Why / rationale:** "Nonsensitive" still means internal-only, but needs no wrapping (DSS §2.3, §5).
- **Source:** DSS §2.3, §5
- **Threat mitigated (STRIDE):** —  ·  **ISO 24772:** —

## Open questions
- None.
