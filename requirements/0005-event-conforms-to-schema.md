---
id: REQ-0005
slug: event-conforms-to-schema
title: Accept an event only if it matches its LogType schema
epic: Schema & Validation
status: Draft
priority: Must
scope: now
verification: test
source: ["DSS §5", "DSS §6.3"]
satisfied_by: []
concepts: [SRP, GuardClause]
stride: [Tampering]
iso24772: [FLC]
user_facing: true
doc_chapter: "Recording events"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an application author, I want Logger to reject events that don't match my declared schema, so that
stored logs stay well-formed and trustworthy.

## Requirement
When an event is submitted for a LogType, the Logger shall accept it only if the event's fields match
the declared field names and types of that LogType.

## Worked example
```
Schema "login": timestamp:Time, ipaddr:IpAddress, user:String
Accepted:  {timestamp: 1234567890, ipaddr: "12.34.56.78", user: "SAM"}
Rejected:  {timestamp: 1234567890, ipaddr: "not-an-ip", user: "SAM"}   (ipaddr wrong type)
Rejected:  {timestamp: 1234567890, user: "SAM"}                        (missing ipaddr)
Rejected:  {timestamp: 1234567890, ipaddr: "12.34.56.78", user: "SAM", extra: "x"}  (unknown field)
```

## Acceptance criteria
- [ ] An event whose fields match the LogType (names + types) is accepted.
- [ ] An event missing a declared field is rejected.
- [ ] An event with a field absent from the schema is rejected.
- [ ] An event with a value that doesn't parse to the declared type is rejected.
- [ ] Submitting an event for an unknown LogType is rejected.

## Design notes
`SchemaValidator.Validate(logType, event)` returning a result (valid / list of violations). Type
checking uses the field types from REQ-0002. Keep validation separate from filtering (SRP).

## Security & traceability
- **Why / rationale:** Only schema-conformant data should enter the log (DSS §5/§6.3); this also
  guards conversion errors when values are later interpreted.
- **Source:** DSS §5, §6.3
- **Threat mitigated (STRIDE):** Tampering (malformed events can't enter the log)  ·  **ISO 24772:** [FLC] conversion errors

## Open questions
- Are unknown extra fields a hard rejection or a warning? (Proposed: rejection, per "must match".)
