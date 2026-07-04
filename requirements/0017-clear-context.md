---
id: REQ-0017
slug: clear-context
title: Clear a context's mappings on request
epic: Pseudonym-context lifecycle
status: Done
priority: Should
scope: now
verification: test
source: ["DSS §8"]
satisfied_by: ["tests/Logger.Core.Tests/PseudonymContextRegistryTests.cs"]
concepts: [SRP]
stride: [InformationDisclosure]
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As an operator, I want to clear my pseudonym mappings for a fresh start, so that a new investigation
isn't coloured by identifiers I established during a previous one.

## Requirement
When a user clears the mappings for a (user, log) context, the Logger shall discard that context's
mappings, so subsequent access re-assigns pseudonyms from scratch.

## Worked example
```
Context (u, login):  SAM -> USER1,  BOB -> USER2
clear(u, login)
Context (u, login):  BOB -> USER1        (fresh — numbering restarts)
```

## Acceptance criteria
- [x] After clearing, the same value is assigned a new (restarted) pseudonym number.
- [x] Clearing one (user, log) context does not affect any other context.

## Design notes
`PseudonymContextRegistry.Clear(user, log)` removes the context entry; the next `GetContext` creates a
fresh `PseudonymContext` (new salt, empty maps). Discarding-and-recreating keeps contexts simple (no
mutable "reset" method needed).

## Security & traceability
- **Why / rationale:** DSS §8 — users "have the ability to clear mappings for a fresh start."
- **Source:** DSS §8
- **Threat mitigated (STRIDE):** InformationDisclosure (stale correlations don't linger)  ·  **ISO 24772:** —

## Open questions
- None.
