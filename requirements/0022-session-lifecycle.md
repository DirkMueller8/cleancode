---
id: REQ-0022
slug: session-lifecycle
title: Enforce the logging session lifecycle order
epic: Session RPC
status: Done
priority: Should
scope: now
verification: test
source: ["DSS §6"]
satisfied_by: ["tests/Logger.Services.Tests/LoggingSessionTests.cs"]
concepts: [StateMachine, SRP]
stride: [Tampering]
iso24772: []
user_facing: true
doc_chapter: "Using the logging API"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As an application author, I want a clear session protocol, so that I connect, declare my schema, log events, and disconnect in a well-defined order that the Logger enforces.

## Requirement
The session shall accept `Hello` first, then any number of `Schema` or `Event` verbs, then `Goodbye`, and shall reject any verb issued out of this order.

## Worked example
```
Hello -> Schema -> Event -> Event -> Goodbye     (accepted)
Event (before Hello)                             (rejected)
Event (before a Schema is defined)               (rejected)
Schema (after Goodbye)                            (rejected)
```

## Acceptance criteria
- [x] `Hello` is accepted only as the first verb; a second `Hello` is rejected.
- [x] `Schema`, `Event`, and `Goodbye` before `Hello` are rejected.
- [x] `Event` before any `Schema` is defined is rejected.
- [x] Any verb after `Goodbye` is rejected.

## Design notes
`LoggingSession` is a state machine (New → Active → Closed). Out-of-order verbs throw
`InvalidOperationException` — a *sequencing/contract* violation, distinct from invalid event *data*
(REQ-0005), which returns a `ValidationResult`. Matching the mechanism to the kind of failure: sequencing errors are exceptions; expected bad data is a result. A real network front-end (out of scope) would translate a thrown sequencing error into an error response.

## Security & traceability
- **Why / rationale:** DSS §6 — the RPC session is `Hello`, then `Schema`/`Event`, then `Goodbye`.
- **Source:** DSS §6
- **Threat mitigated (STRIDE):** Tampering (a malformed verb sequence can't corrupt session state)  ·  **ISO 24772:** —

## Open questions
- None.
