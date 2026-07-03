---
id: REQ-0004
slug: disposition-mandatory-per-field
title: Every field must declare a disposition
epic: Schema & Validation
status: Draft
priority: Must
scope: now
verification: test
source: ["DSS §5"]
satisfied_by: []
concepts: [SRP, ValueObject]
stride: [InformationDisclosure]
iso24772: []
user_facing: true
doc_chapter: "Defining what you log"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As a privacy owner, I want to be forced to declare the sensitivity of every field, so that no data item is logged without a deliberate decision about how it may be exposed.

## Requirement
The Logger shall require every field in a schema to declare a disposition (one of `nonsensitive`, `private`, or a named filter), and shall reject any schema with a field whose disposition is missing.

## Worked example
```
Rejected:  user:String                    (no disposition)
Accepted:  user:String  disposition=private
           ipaddr:IpAddress disposition=country
           http:String  disposition=nonsensitive
```

## Acceptance criteria
- [ ] A field with a valid disposition is accepted.
- [ ] A field with no disposition is rejected.
- [ ] A field naming an unknown disposition is rejected (validated against the registered filters, REQ-0013).
- [ ] The rejection identifies the offending field.

## Design notes
`Disposition` value object: either a well-known kind (`Nonsensitive`, `Private`) or a named custom filter resolved via the filter registry (REQ-0013). Validation belongs with schema construction.
**Consequence of the resolved question below:** schema construction is validated *against the filter
registry*, so building a schema takes the registry as a dependency (the builder/factory receives it).
An unknown disposition name fails at build time, not at first use.

## Security & traceability
- **Why / rationale:** DSS §5 makes declaring disposition mandatory precisely to force an explicit, reviewed choice per field — the opposite of accidental disclosure.
- **Source:** DSS §5
- **Threat mitigated (STRIDE):** InformationDisclosure (no un-classified field can be logged)  ·  **ISO 24772:** —

## Open questions
- ~~Validate a named custom disposition at build time or first use?~~ **Resolved (2026-07-03):** at
  **schema-build time**, so bad schemas fail fast. Schema construction therefore depends on the filter
  registry (see Design notes).
