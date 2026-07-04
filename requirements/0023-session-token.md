---
id: REQ-0023
slug: session-token
title: Issue a high-entropy session token on Hello
epic: Session RPC
status: Done
priority: Could
scope: now
verification: test
source: ["DSS §6.1"]
satisfied_by: ["tests/Logger.Services.Tests/LoggingSessionTests.cs", "tests/Logger.Services.Tests/StoreAndTokenTests.cs"]
concepts: [DIP]
stride: [Spoofing]
iso24772: []
user_facing: true
doc_chapter: "Using the logging API"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As a security owner, I want session tokens to be unguessable, so that another process can't hijack a logging session by guessing its token.

## Requirement
When `Hello` is accepted, the session shall issue a token with at least 120 bits of entropy.

## Worked example
```
Hello -> token "9F2A…"   (>= 120 bits; DSS recommends ~20 base64 chars minimum)
```

## Acceptance criteria
- [x] A token issued on `Hello` carries at least 120 bits of entropy.
- [x] Two issued tokens differ (they are random, not sequential).
- [x] The token generator is injected, not hard-coded (so tests can use a predictable stub).

## Design notes
`ITokenGenerator` seam; `SecureTokenGenerator` uses `RandomNumberGenerator` to produce 128 bits
(16 bytes, above the 120-bit minimum) rendered as hex. Injected so lifecycle tests can substitute a deterministic token.

## Security & traceability
- **Why / rationale:** DSS §6.1 — "tokens are generated randomly with sufficient complexity and entropy to preclude guessing: the minimum recommended token size is 120 bits."
- **Source:** DSS §6.1
- **Threat mitigated (STRIDE):** Spoofing (session hijack by token guessing)  ·  **ISO 24772:** —

## Open questions
- None.
