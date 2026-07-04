---
id: REQ-0002
slug: define-log-schema
title: Define a log schema of typed fields
epic: Schema & Validation
status: Done
priority: Must
scope: now
verification: test
source: ["DSS §5", "DSS §6.3"]
satisfied_by: ["tests/Logger.Core.Tests/SchemaTests.cs"]
concepts: [SRP, ValueObject]
stride: [Tampering]
iso24772: []
user_facing: true
doc_chapter: "Defining what you log"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an application author, I want to declare the shape of my log events up front, so that Logger knows which fields exist and how each should be handled.

## Requirement
The Logger shall represent a schema as a set of named LogTypes, where each LogType declares a set of named, typed fields.

## Worked example
```
Schema:
  LogType "login"  -> fields: timestamp:Time, ipaddr:IpAddress, user:String, password:String
  LogType "logout" -> fields: timestamp:Time, ipaddr:IpAddress, user:String
```

## Acceptance criteria
- [x] A schema can hold multiple LogTypes, each retrievable by name.
- [x] A LogType exposes its fields, each with a name and a type.
- [x] The supported field types are exactly `Time`, `IpAddress`, `String`, and `Integer`.
- [x] Two LogTypes with the same name in one schema are rejected.
- [x] A field name duplicated within one LogType is rejected.
- [x] Field and log-type names are compared **case-sensitively**: `"user"` and `"User"` are
      distinct names (not a duplicate). See [glossary](glossary.md) → *name*.

## Design notes
Immutable value objects: `Schema` → `LogType` → `FieldDefinition(name, type, disposition)`. Build via
a validating factory/builder so an invalid schema cannot be constructed. Field disposition is added by REQ-0004; this requirement covers structure only.

## Security & traceability
- **Why / rationale:** An explicit, declared schema is the foundation for making a deliberate sensitivity decision about every field (DSS §5) rather than logging data ad hoc.
- **Source:** DSS §5, §6.3
- **Threat mitigated (STRIDE):** Tampering (well-formed, validated structure)  ·  **ISO 24772:** —

## Open questions
- ~~Which built-in field types do we need for the core?~~ **Resolved (2026-07-03):** exactly
  `Time`, `IpAddress`, `String`, `Integer`. Modelled as a closed set (e.g. an enum or sealed type hierarchy) so an unknown type can't be declared.
- ~~Are names compared case-sensitively or case-insensitively?~~ **Resolved (2026-07-03):
  case-sensitive.** Raised *after* implementation: the code had silently defaulted to an ordinal (case-sensitive) comparison, which is an observable/functional behaviour no acceptance criterion
  pinned. Lifted back into the spec as an explicit criterion + glossary definition rather than left as an incidental implementation detail. Case-sensitive matches JSON key semantics and avoids a filter quietly merging two distinct fields; revisit if a real schema needs case-insensitive names.
