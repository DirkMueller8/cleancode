---
id: REQ-0020
slug: forbid-exact-match-on-filtered-fields
title: Forbid exact-value queries on pseudonymized fields
epic: Query over filtered logs
status: Done
priority: Should
scope: now
verification: test
source: ["DSS §7"]
satisfied_by: ["tests/Logger.Core.Tests/FilteredLogQueryTests.cs"]
concepts: [SRP, SecurityByDesign]
stride: [InformationDisclosure]
iso24772: [XZL]
user_facing: true
doc_chapter: "Searching the logs"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As a privacy owner, I want the log search to refuse guesses at hidden values, so that nobody can
confirm a private value (an IP, a username) just by querying candidate values until one matches.

## Requirement
If a query specifies a raw (non-symbol) value for a pseudonymized field, then the Logger shall reject
the query rather than evaluate it.

## Worked example
```
Allowed:   [ipaddr = US1]        (a symbol the user can already see)
Rejected:  [ipaddr = 1.1.1.1]    (a raw guess — could confirm the hidden value if it matched)
Allowed:   [http = POST]         (http is nonsensitive — exact values are fine)
```

## Acceptance criteria
- [x] A query giving a raw/non-symbol value for a pseudonymized field is rejected.
- [x] A query giving a valid symbol (`US1`) for a pseudonymized field is allowed.
- [x] A non-symbol exact value on a nonsensitive field is allowed (not rejected).

## Design notes
The `FilteredLogQueryEngine` is given the set of pseudonymized field names (the service layer derives
this from the schema's dispositions — `private`, `country`, custom pseudonym filters). For those fields,
a condition value must match the **symbol** shape (uppercase letters then digits, e.g. `US1`); anything
else is treated as a raw guess and the query is rejected. The symbol shape is a documented heuristic for
the core. This is *security by design* — the unsafe query is unrepresentable, not merely discouraged.

## Security & traceability
- **Why / rationale:** DSS §7 — searches on filtered fields with exact values must be disallowed, or a
  user could "guess `[IP = 1.1.1.1]` … eventually hit a log entry … and infer the actual value."
- **Source:** DSS §7
- **Threat mitigated (STRIDE):** InformationDisclosure (inference/oracle attack)  ·  **ISO 24772:** [XZL] discrepancy information leak

## Open questions
- None.
