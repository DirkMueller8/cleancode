---
id: REQ-0024
slug: token-invalidation
title: Invalidate the session token on Goodbye
epic: Session RPC
status: Done
priority: Should
scope: now
verification: test
source: ["DSS §6.4"]
satisfied_by: ["tests/Logger.Services.Tests/LoggingSessionTests.cs"]
concepts: [StateMachine]
stride: [Spoofing]
iso24772: []
user_facing: true
doc_chapter: "Using the logging API"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As a security owner, I want a session's token to stop working once the session ends, so that a leaked token can't be replayed after `Goodbye`.

## Requirement
When `Goodbye` is processed, the session shall invalidate its token; resuming logging requires a new `Hello`.

## Worked example
```
Hello -> token issued, session Active
Goodbye -> token invalidated, session Closed
Event (after Goodbye) -> rejected (needs a new Hello)
```

## Acceptance criteria
- [x] After `Goodbye`, the session holds no valid token and is no longer active.
- [x] Any verb after `Goodbye` is rejected (ties REQ-0022).

## Design notes
`Goodbye` moves the session `Active → Closed` and clears the token. A closed session is terminal — a new `LoggingSession` (and `Hello`) is required to log again.

## Security & traceability
- **Why / rationale:** DSS §6.4 — after `Goodbye`, "the token thereafter is no longer valid. To resume logging, the client must first make a `Hello` request."
- **Source:** DSS §6.4
- **Threat mitigated (STRIDE):** Spoofing (no token replay after session end)  ·  **ISO 24772:** —

## Open questions
- None.
