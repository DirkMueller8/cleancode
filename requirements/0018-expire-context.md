---
id: REQ-0018
slug: expire-context
title: Auto-expire a context after 24h idle
epic: Pseudonym-context lifecycle
status: Done
priority: Should
scope: now
verification: test
source: ["DSS §8"]
satisfied_by: ["tests/Logger.Core.Tests/PseudonymContextRegistryTests.cs"]
concepts: [DIP, SRP]
stride: [InformationDisclosure]
iso24772: [CCI]
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As a privacy owner, I want unused pseudonym mappings to expire automatically, so that identifier maps
don't accumulate or persist longer than they're needed.

## Requirement
If a (user, log) context has not been accessed for 24 hours, then the Logger shall discard it, so its
mappings do not persist indefinitely.

## Worked example
```
Context (u, login) last accessed at 09:00 on day 1
... 24h+ of no access ...
Next access -> a fresh context (previous mappings gone)
```

## Acceptance criteria
- [x] A context idle for at least 24h is discarded; the next access starts fresh.
- [x] A context idle for less than 24h is retained (its mappings persist).
- [x] The current time is obtained from an injected clock, not read from the system directly.

## Design notes
`PseudonymContextRegistry` records each context's last-access time using an injected `IClock`
(`SystemClock` in production, a controllable fake in tests). On access it purges any context whose idle
time is `>= 24h`. Injecting the clock is what makes 24h expiry testable without waiting — and avoids
reading the system clock directly (ISO 24772 `[CCI]`).

## Security & traceability
- **Why / rationale:** DSS §8 — after 24 hours of non-use, mappings "are automatically cleared to
  prevent useless buildup over time."
- **Source:** DSS §8
- **Threat mitigated (STRIDE):** InformationDisclosure (bounded retention of correlations)  ·  **ISO 24772:** [CCI] clock issues

## Open questions
- None.
