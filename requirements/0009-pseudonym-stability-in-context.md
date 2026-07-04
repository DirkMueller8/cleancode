---
id: REQ-0009
slug: pseudonym-stability-in-context
title: Map equal values to equal pseudonyms within a context
epic: Filtering Engine
status: Done
priority: Must
scope: now
verification: test
source: ["DSS §2.3", "DSS §8"]
satisfied_by: ["tests/Logger.Core.Tests/PseudonymContextTests.cs", "tests/Logger.Core.Tests/PrivateFilterTests.cs"]
concepts: [DIP]
stride: [InformationDisclosure]
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an operator, I want the same hidden value to always show the same pseudonym, so that I can
correlate related events (e.g. "all requests from IP `US7`") without seeing the real value.

## Requirement
While within one pseudonym context, the Logger shall assign equal raw values the same pseudonym and
distinct raw values distinct pseudonyms.

## Worked example
```
Within one context:
  events with IP 66.77.88.99  -> all shown as US1(v4)
  event   with IP 10.0.0.1    -> shown as US2(v4)
Correlation: querying US1 finds every event from 66.77.88.99 without revealing it.
```

## Acceptance criteria
- [x] Filtering the same value twice in one context yields the identical pseudonym.
- [x] Filtering two different values in one context yields different pseudonyms.
- [x] Sequence numbers increase in first-seen order within the context.

## Design notes
The context owns a value→pseudonym map (keyed by digest, REQ-0010). `IPseudonymContext` is the DIP
seam the filters depend on. Cross-context behavior is REQ-0016; this requirement is stability *within*
one context.

## Security & traceability
- **Why / rationale:** Consistent pseudonyms enable legitimate correlation while withholding the value
  (DSS §2.3, §8).
- **Source:** DSS §2.3, §8
- **Threat mitigated (STRIDE):** InformationDisclosure (correlation without disclosure)  ·  **ISO 24772:** —

## Open questions
- None.
