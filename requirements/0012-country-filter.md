---
id: REQ-0012
slug: country-filter
title: Map IP addresses to country of origin in the filtered view
epic: Filtering Engine
status: Draft
priority: Should
scope: now
verification: test
source: ["DSS §5", "DSS §2.3"]
satisfied_by: []
concepts: [Strategy, DIP]
stride: [InformationDisclosure]
iso24772: []
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an operator, I want IP addresses reduced to their country in filtered views, so that I can reason
about geography (and still correlate) without exposing individual addresses.

## Requirement
Where a field's disposition is `country`, the Logger shall replace the IP address with a pseudonym
combining its country and a per-context sequence number, plus an address-family hint.

## Worked example
```
Raw:      66.77.88.99
Filtered: US1(v4)          (first distinct US address in this context, IPv4)
```

## Acceptance criteria
- [ ] An IPv4 address in the US renders as `US<n>(v4)`.
- [ ] The country is obtained from an injected lookup, not hard-coded.
- [ ] Two addresses in the same country get the country prefix but distinct sequence numbers.
- [ ] The address family hint reflects IPv4 vs IPv6.

## Design notes
`CountryFilter : IFieldFilter` depends on `IGeoLookup { string CountryOf(IpAddress ip); }` (DIP seam,
stubbed in tests) and on `IPseudonymContext` for the sequence number. Combines the pseudonym
mechanics of REQ-0008/0009 with an external lookup — a good second DIP example.

## Security & traceability
- **Why / rationale:** DSS §2.3/§5 use `country` (`US1`) as a built-in filter that keeps geography
  useful while hiding the address.
- **Source:** DSS §5, §2.3
- **Threat mitigated (STRIDE):** InformationDisclosure (address withheld, geography retained)  ·  **ISO 24772:** —

## Open questions
- Where does the address-family hint live — on the pseudonym here, or generalized into REQ-0008's hint
  policy? (Proposed: reuse the same hint mechanism.)
