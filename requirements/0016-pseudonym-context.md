---
id: REQ-0016
slug: pseudonym-context
title: Maintain pseudonym mappings separately per (user, log) context
epic: Pseudonym-context lifecycle
status: Done
priority: Must
scope: now
verification: test
source: ["DSS §8"]
satisfied_by: ["tests/Logger.Core.Tests/PseudonymContextTests.cs"]
concepts: [SRP, DIP]
stride: [InformationDisclosure]
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As a privacy owner, I want each viewer's pseudonym mappings kept separate, so that one user's view of
the logs never leaks correlations into another's, and a view can be reset independently.

## Requirement
The Logger shall maintain pseudonym mappings separately per (user, log) context, so that allocations
made in one context do not affect any other context.

## Worked example
```
Context A:  SAM -> USER1,  BOB -> USER2
Context B:  BOB -> USER1            (independent — B has its own counters and salt)
```

## Acceptance criteria
- [x] Each context assigns sequence numbers from its own counters (a value is numbered independently
      in a different context).
- [x] Allocations in one context do not change another context's numbering.
- [x] Each context uses its own salt (so equal values in different contexts need not share a digest).

## Design notes
`IPseudonymContext` is the abstraction filters depend on (DIP); `PseudonymContext` is one instance per
(user, log) view — isolation is achieved by construction (separate instances hold separate state).
Stability *within* a context is REQ-0009; salted-digest storage is REQ-0010.

**Scope note:** this requirement covers the context abstraction and its per-context isolation only.
*Keying* contexts by (user, log) and their **lifecycle** — manual clear (REQ-0017) and 24h-idle expiry
(REQ-0018) — are separate requirements. Note pseudonym *strings* may coincide across contexts (both may
start at `USER1`); isolation is about independence of state, not visual difference.

## Security & traceability
- **Why / rationale:** DSS §8 — the pseudonym mapping is "maintained separately for each user context
  per log," so correlations don't leak across viewers and a view can be cleared independently.
- **Source:** DSS §8
- **Threat mitigated (STRIDE):** InformationDisclosure (no cross-viewer correlation leak)  ·  **ISO 24772:** —

## Open questions
- None. (Lifecycle/keying deferred to REQ-0017 / REQ-0018.)
