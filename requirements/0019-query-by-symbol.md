---
id: REQ-0019
slug: query-by-symbol
title: Query filtered logs by symbolic identifier
epic: Query over filtered logs
status: Done
priority: Could
scope: now
verification: test
source: ["DSS §7"]
satisfied_by: ["tests/Logger.Core.Tests/FilteredLogQueryTests.cs"]
concepts: [SRP]
stride: []
iso24772: []
user_facing: true
doc_chapter: "Searching the logs"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As an operator, I want to search filtered logs by the symbolic identifiers I can see (like `US1`), so
that I can correlate related events without ever knowing the real hidden values.

## Requirement
When a query references a field by a symbolic identifier, the Logger shall return the filtered entries
whose filtered value for that field carries that identifier (ignoring the format/length hint).

## Worked example
```
Entries:  { ipaddr: US1(v4), http: POST }   { ipaddr: US2(v4), http: GET }
Query:    [ipaddr = US1]           ->  first entry only
Query:    [http = POST]            ->  first entry only (nonsensitive, exact)
```

## Acceptance criteria
- [x] A query `[field = US1]` matches entries whose filtered value has the identifier `US1` (e.g. `US1(v4)`).
- [x] The format/length hint is ignored in matching (`US1` matches `US1(v4)`).
- [x] A non-matching identifier (`US2`) does not match.
- [x] Multiple conditions are combined with AND.
- [x] A nonsensitive field matches on its exact value.

## Design notes
`FilteredLogQueryEngine.Search(entries, conditions)` over `FilteredView` entries. A value's *symbol* is
the part before the `(` hint. Matching compares condition value to the entry's symbol. The engine works
on already-filtered entries only (it never touches raw data).

## Security & traceability
- **Why / rationale:** DSS §7 — "a query of [IP = US1] would find other logs from that IP address
  without disclosing the address itself."
- **Source:** DSS §7
- **Threat mitigated (STRIDE):** —  ·  **ISO 24772:** —

## Open questions
- None.
