---
id: REQ-0005
slug: event-conforms-to-schema
title: Accept an event only if it matches its LogType schema
epic: Schema & Validation
status: Done
priority: Must
scope: now
verification: test
source: ["DSS §5", "DSS §6.3"]
satisfied_by: ["tests/Logger.Core.Tests/EventValidationTests.cs"]
concepts: [SRP, GuardClause, DIP]
stride: [Tampering]
iso24772: [FLC, OYB]
user_facing: true
doc_chapter: "Recording events"
created: 2026-07-03
updated: 2026-07-04
---

## Summary
As an application author, I want Logger to reject events that don't match my declared schema, so that stored logs stay well-formed and trustworthy.

## Requirement
When an event is submitted for a LogType, the Logger shall accept it only if the event's fields match the declared field names and types of that LogType.

## Worked example
```
Schema "login": timestamp:Time, ipaddr:IpAddress, user:String
Accepted:  {timestamp: 1234567890, ipaddr: "12.34.56.78", user: "SAM"}
Rejected:  {timestamp: 1234567890, ipaddr: "not-an-ip", user: "SAM"}   (ipaddr wrong type)
Rejected:  {timestamp: 1234567890, user: "SAM"}                        (missing ipaddr)
Rejected:  {timestamp: 1234567890, ipaddr: "12.34.56.78", user: "SAM", extra: "x"}  (unknown field)
```

## Acceptance criteria
- [x] An event whose fields match the LogType (names + types) is accepted.
- [x] An event missing a declared field is rejected.
- [x] An event with a field absent from the schema is rejected.
- [x] An event with a value that doesn't parse to the declared type is rejected.
- [x] Submitting an event for an unknown LogType is rejected.

## Design notes
`SchemaValidator(schema).Validate(logEvent)` returns a `ValidationResult` (valid / list of violations).
Type checking uses the field types from REQ-0002. Keep validation separate from filtering (SRP).

**As built (2026-07-04) — decisions surfaced:**
- *Return a result, don't throw.* Invalid schemas throw (rare, programmer error, build-time); invalid
  *events* are an **expected** runtime outcome (untrusted, high-volume), so `Validate` returns a
  `ValidationResult` the caller must inspect — using exceptions for expected control flow would be a
  design + performance smell. Forces the caller not to ignore the status (ISO 24772 `[OYB]`).
- *Collect all violations,* not just the first — more useful to the caller (one test proves aggregation).
- *Event values are text.* A `LogEvent` holds field values as strings (DSS §8: canonical text form);
  the validator parses each against its declared type (`[FLC]`).
- *`Time` validates as Unix epoch seconds* (matches the API example `timestamp: 1234567890`). This is
  the canonical stored form; REQ-0011's `minute` filter will round it. Documented so it isn't incidental.

## Security & traceability
- **Why / rationale:** Only schema-conformant data should enter the log (DSS §5/§6.3); this also guards conversion errors when values are later interpreted.
- **Source:** DSS §5, §6.3
- **Threat mitigated (STRIDE):** Tampering (malformed events can't enter the log)  ·  **ISO 24772:** [FLC] conversion errors

## Open questions
- ~~Unknown extra fields — reject or warn?~~ **Resolved (2026-07-03):** hard **rejection**, per "must
  match". An event carrying any field absent from the schema is rejected (already covered by the
  acceptance criteria).
