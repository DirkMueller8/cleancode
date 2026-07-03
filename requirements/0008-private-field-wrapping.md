---
id: REQ-0008
slug: private-field-wrapping
title: Wrap private fields as pseudonyms with a format/length hint
epic: Filtering Engine
status: Draft
priority: Must
scope: now
verification: test
source: ["DSS §2.3", "DSS §8"]
satisfied_by: []
concepts: [Strategy, OCP]
stride: [InformationDisclosure]
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an operator, I want private fields shown as pseudonyms with a small format hint, so that I can
work with the logs without ever seeing the raw private value.

## Requirement
Where a field's disposition is `private`, the Logger shall replace its value in the filtered view with
a pseudonym composed of a type prefix, a per-context sequence number, and a format/length hint.

## Worked example
```
Raw:      user: "SAM",  password: ">1<}2{]3[\4/"
Filtered: user: USER1(3), password: PW1(12)
```
`USER1(3)` = username field, first distinct value in this context, 3 characters long.

## Acceptance criteria
- [ ] A private string yields `<prefix><n>(<length>)` (e.g. a 3-char username → `USER1(3)`).
- [ ] The raw value never appears anywhere in the filtered output (assert not-contains).
- [ ] The length hint equals the character length of the raw value.
- [ ] The type prefix is derived from the field (e.g. `user` → `USER`, `password` → `PW`).
- [ ] Sequence numbering is per pseudonym-context (see REQ-0009 / REQ-0016).

## Design notes
`PrivateFilter : IFieldFilter`. Needs the `IPseudonymContext` (REQ-0016) to allocate/lookup the
sequence number for a value; stability is REQ-0009; digest-based storage is REQ-0010. Prefix mapping
is a small, replaceable policy (keep it open for extension).

## Security & traceability
- **Why / rationale:** Minimizes exposure while preserving correlation and a useful shape hint
  (an over-long password is visible from its length alone) — DSS §2.3.
- **Source:** DSS §2.3, §8
- **Threat mitigated (STRIDE):** InformationDisclosure (raw private value withheld)  ·  **ISO 24772:** —

## Open questions
- ~~How is the type prefix chosen?~~ **Resolved (2026-07-03):** a small default policy maps the field
  name to a prefix (`user`→`USER`, `password`→`PW`), falling back to the uppercased field name for
  unmapped fields. A per-field override may be added later. The mapping is a replaceable policy
  (`IPrefixPolicy`), kept open for extension.
