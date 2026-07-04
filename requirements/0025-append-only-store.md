---
id: REQ-0025
slug: append-only-store
title: Persist events to an append-only store (in-memory stub)
epic: Storage
status: Done
priority: Should
scope: now
verification: test
source: ["DSS §5", "DSS §8"]
satisfied_by: ["tests/Logger.Services.Tests/StoreAndTokenTests.cs"]
concepts: [DIP, SRP]
stride: [Tampering]
iso24772: []
user_facing: false
doc_chapter: "Storage"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As an auditor, I want recorded events to be stored append-only, so that the log is a reliable, non-repudiable record that isn't quietly modified after the fact.

## Requirement
The Logger shall persist recorded events to an append-only store: events can be appended and read, but never modified or removed (other than policy expiry, which is out of scope here).

## Worked example
```
Append(event A); Append(event B)
All() -> [A, B]   (in append order; no update/delete API exists)
```

## Acceptance criteria
- [x] An appended event is readable, in append order.
- [x] The store exposes no way to modify or remove an existing event.

## Design notes
`ILogStore` seam; `InMemoryLogStore` holds events in a list. Append-only is enforced by the API shape —
there simply is no update/delete method (illegal states unrepresentable). Real persistence (files, AES at rest, RAID) is out of scope; this is the in-memory stub for the "core + service stubs" scope.

## Security & traceability
- **Why / rationale:** DSS §5/§8 — "logs are append-only records of software events and are never   modified other than being deleted upon expiration."
- **Source:** DSS §5, §8
- **Threat mitigated (STRIDE):** Tampering (no after-the-fact edits)  ·  **ISO 24772:** —

## Open questions
- None.
