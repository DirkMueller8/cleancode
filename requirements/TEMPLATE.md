---
id: REQ-NNNN
slug: short-kebab-title
title: <short human title>
epic: <epic/feature name — groups the index and the user-guide chapter>
status: Draft            # Draft | In progress | Done | Parked
priority: Must           # MoSCoW: Must | Should | Could | Wont
scope: now               # now | later | out-of-scope-infra
verification: test       # test | demo | inspection | analysis
source: []               # e.g. ["DSS §2.3", "DSS §8"] — where this comes from
satisfied_by: []         # e.g. ["tests/Logger.Core.Tests/PrivateFilterTests.cs"] — filled when built
concepts: []             # e.g. [Strategy, OCP, DIP] — what this exercises
stride: []               # STRIDE threats mitigated: [Spoofing, Tampering, Repudiation, InformationDisclosure, DoS, ElevationOfPrivilege]
iso24772: []             # relevant ISO/IEC 24772-1 codes, e.g. [MVX, CCI, XZP]
user_facing: true        # does this feed the generated user guide?
doc_chapter: <chapter>   # user-guide section this lands in (when user_facing)
created: YYYY-MM-DD
updated: YYYY-MM-DD
---

<!-- Sections are tagged (U) user-facing → feeds the user guide, or (I) internal → engineers only.
     The doc generator emits only user_facing requirements and only their (U) sections. -->

## Summary  <!-- (U) -->
As a `<role>`, I want `<capability>`, so that `<benefit>`.

## Requirement  <!-- (U) — the single normative statement; EARS encouraged -->
<!-- Pick an EARS pattern: Ubiquitous / When / While / Where / If…then. One `shall`. -->
The `<system>` shall `<response>`.

## Worked example  <!-- (U) — include only when user_facing; concrete input → output -->
```
Input:
Output:
```

## Acceptance criteria  <!-- (I) — each phrased so exactly one test can pass/fail it -->
- [ ]
- [ ]

## Design notes  <!-- (I) — interfaces/collaborators; which SOLID principle this exercises -->
-

## Security & traceability  <!-- (I) -->
- **Why / rationale:**
- **Source:** <mirrors frontmatter `source`>
- **Threat mitigated (STRIDE):** <mirrors `stride`>  ·  **ISO 24772:** <mirrors `iso24772`>

## Open questions  <!-- (I) — resolve before Build -->
-
