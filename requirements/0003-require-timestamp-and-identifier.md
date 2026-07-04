---
id: REQ-0003
slug: require-timestamp-and-identifier
title: Require a timestamp and at least one other identifying field
epic: Schema & Validation
status: Done
priority: Must
scope: now
verification: test
source: ["DSS §5"]
satisfied_by: ["tests/Logger.Core.Tests/TimestampAndIdentifierTests.cs"]
concepts: [SRP]
stride: [Repudiation]
iso24772: []
user_facing: true
doc_chapter: "Defining what you log"
created: 2026-07-03
updated: 2026-07-04
---

## Summary
As an auditor, I want every log event to carry a timestamp and at least one identifying field, so that records are attributable and form a reliable audit trail.

## Requirement
If a LogType does not declare a timestamp field and at least one additional identifying field, then the Logger shall reject the schema.

## Worked example
```
Rejected:  LogType "ping" -> fields: timestamp:Time (no other identifying field)
Accepted:  LogType "ping" -> fields: timestamp:Time, ipaddr:IpAddress
```

## Acceptance criteria
- [x] A LogType with a timestamp and ≥1 other field is accepted.
- [x] A LogType with only a timestamp is rejected.
- [x] A LogType with no timestamp is rejected.
- [x] The rejection identifies the offending LogType by name.

## Design notes
Enforced in the same schema-building validation as REQ-0002/0004; a distinct rule so it maps to its own tests. "Identifying field" = any declared field other than the timestamp (kept simple for the core).

**As built (2026-07-04) — two decisions surfaced (spec was silent on both):**
- *What is "the timestamp"?* Identified by **type** (`FieldType.Time`), not by a field named
  `"timestamp"`. So the time field may be named anything; a `String` called `"timestamp"` is not one.
- *Multiple `Time` fields* (e.g. `start`, `end`): **accepted** — a second `Time` field satisfies the
  "at least one other field" rule, per this requirement's resolved reading. Covered by a test.
- *Placement:* validated at `Schema` construction (matches "reject the schema"), leaving `LogType` the
  pure structural type from REQ-0002 — so a bare/degenerate `LogType` can still be built in isolation,
  it just can't enter a schema.

## Security & traceability
- **Why / rationale:** A non-repudiable record needs a time and a subject; DSS §5 requires "a timestamp and at least one other identifying data item."
- **Source:** DSS §5
- **Threat mitigated (STRIDE):** Repudiation (attributable, time-stamped records)  ·  **ISO 24772:** —

## Open questions
- ~~Should specific field(s) be markable as "the" identifier?~~ **Resolved (2026-07-03):** no — an "identifying field" is *any* declared field other than the timestamp. No per-field identifier flag in the core.
