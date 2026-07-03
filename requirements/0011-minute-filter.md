---
id: REQ-0011
slug: minute-filter
title: Round timestamps to the minute in the filtered view
epic: Filtering Engine
status: Draft
priority: Should
scope: now
verification: test
source: ["DSS §5"]
satisfied_by: []
concepts: [Strategy]
stride: [InformationDisclosure]
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an operator, I want timestamps coarsened to the minute in filtered views, so that precise timing
can't be used to single out or correlate individuals unnecessarily.

## Requirement
Where a field's disposition is `minute`, the Logger shall round its timestamp down to the start of the
containing minute in the filtered view.

## Worked example
```
Raw:      2022/10/19 08:09:10
Filtered: 2022/10/19 08:09
```

## Acceptance criteria
- [ ] Seconds and sub-second precision are removed (floored, not rounded to nearest).
- [ ] `08:09:59` → `08:09`; `08:10:00` → `08:10`.
- [ ] The unfiltered view retains the full-precision timestamp.

## Design notes
`MinuteFilter : IFieldFilter`, a second "built-in" strategy operating on a Time value. Pure; no
context needed. Confirms the Strategy seam works for non-string, non-wrapped transforms.

## Security & traceability
- **Why / rationale:** Coarsening obscures precise times (DSS §5 lists `minute` as a built-in filter).
- **Source:** DSS §5
- **Threat mitigated (STRIDE):** InformationDisclosure (timing precision reduced)  ·  **ISO 24772:** —

## Open questions
- ~~Floor vs nearest?~~ **Resolved (2026-07-03): floor.** This is a *deliberate deviation* from
  DSS §5 ("nearest minute"): flooring is deterministic and never reveals a later minute than the event.
  Recorded as an intentional, reviewed departure from the source, not an oversight.
