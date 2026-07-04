---
id: REQ-0006
slug: size-and-encoding-limits
title: Enforce request and field size limits
epic: Schema & Validation
status: Done
priority: Should
scope: now
verification: test
source: ["DSS §5"]
satisfied_by: ["tests/Logger.Core.Tests/SizeLimitTests.cs"]
concepts: [GuardClause, Reuse]
stride: [DoS]
iso24772: [XZP, EFS]
user_facing: true
doc_chapter: "Recording events"
created: 2026-07-03
updated: 2026-07-04
---

## Summary
As an operator, I want oversized inputs rejected, so that a single request cannot exhaust resources or smuggle in unbounded data.

## Requirement
If a request exceeds 1,000,000 characters, or any individual field value exceeds 10,000 characters, then the Logger shall reject it.

## Worked example
```
Accepted:  user value of 12 chars
Rejected:  user value of 10,001 chars          (field limit)
Rejected:  total request of 1,000,001 chars    (request limit)
```

## Acceptance criteria
- [x] A field value of exactly 10,000 chars is accepted; 10,001 is rejected.
- [x] A request of exactly 1,000,000 chars is accepted; 1,000,001 is rejected.
- [x] The rejection states which limit was exceeded (and the field, for field limits).
- [x] Length is measured in characters of the decoded UTF-8 text.

## Design notes
Boundary-value tests are the point here. Constants in one place (`Limits`). Applied at the input boundary before schema validation. Encoding validity (valid UTF-8/JSON) can be asserted by the service layer; the core assumes decoded text.

**As built (2026-07-04):**
- *Reuse:* returns the same `ValidationResult` type as REQ-0005 (result, not throw — an oversized input
  is an expected hostile request, not a programmer error).
- *Two checks, two inputs:* `SizeValidator.ValidateRequestSize(rawText)` (the 1M limit is about the raw
  serialized request the service layer supplies) and `ValidateFieldSizes(logEvent)` (the 10k per-field
  limit on the parsed event). Deliberately not enforced in `LogEvent`'s constructor — construction stays
  total, validation stays separate (SRP).
- *"Characters" caveat:* measured as C# `string.Length` (UTF-16 code units); equals the Unicode
  character count except for astral-plane chars (emoji). Acceptable at this coarse limit; documented so
  it isn't incidental.

## Security & traceability
- **Why / rationale:** Bounds at the boundary prevent resource exhaustion and constrain tainted input (DSS §5 states the 1M / 10k limits).
- **Source:** DSS §5
- **Threat mitigated (STRIDE):** DoS  ·  **ISO 24772:** [XZP] resource exhaustion, [EFS] tainted input

## Open questions
- None.
