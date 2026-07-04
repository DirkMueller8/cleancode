---
id: REQ-0015
slug: unfiltered-view
title: Provide the unfiltered (raw) view to authorized callers
epic: Filtering Engine
status: Done
priority: Could
scope: now
verification: test
source: ["DSS §2.3", "DSS §5"]
satisfied_by: ["tests/Logger.Core.Tests/UnfilteredViewTests.cs"]
concepts: [SRP, DIP]
stride: [InformationDisclosure, ElevationOfPrivilege]
iso24772: [OYB]
user_facing: true
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As an authorized investigator, I want to retrieve the raw event when strictly necessary, so that I can diagnose incidents that the filtered view can't resolve.

## Requirement
Where a caller is authorized for unfiltered access, the Logger shall return the raw event values; if the caller is not authorized, then the Logger shall deny the request.

## Worked example
```
Authorized:   returns {user: "SAM", password: ">1<}2{]3[\4/"}
Unauthorized: request denied (no raw values returned)
```

## Acceptance criteria
- [x] An authorized caller receives the exact raw values.
- [x] An unauthorized caller is denied and receives no raw values.
- [x] The authorization decision is made through an injected policy, not hard-coded.

## Design notes
`IUnfilteredAccessPolicy { bool IsAllowed(AccessRequest request); }` — a stub in the core; the real approval/token workflow (Epic H) is out of scope now. Keep the raw-return path separate from filtering (SRP). Full auth, tokens, and audit logging of the access come later.

**As built (2026-07-04):** `UnfilteredViewProvider(IUnfilteredAccessPolicy)` reveals values only when
the policy allows (deny by default). The result is an `UnfilteredAccessResult`: a **denied result holds
no values and reading `.Values` throws**, so raw data cannot leak even from a caller that forgets to
check `IsGranted` — the safety is structural, not a convention (`[OYB]`). Return-a-result (not
throw-on-deny) because denial is an expected outcome. `AccessRequest` carries user + log only; scope,
time window, reason, and audit logging are Epic H.

## Security & traceability
- **Why / rationale:** DSS §2.3 permits unfiltered access only "in a controlled manner with proper authorization." Denying by default and never leaking raw values on denial is the key rule.
- **Source:** DSS §2.3, §5
- **Threat mitigated (STRIDE):** InformationDisclosure, ElevationOfPrivilege  ·  **ISO 24772:** [OYB] (don't ignore a failed authorization check)

## Open questions
- ~~Is deferring the audit-log-the-access requirement (DSS §3) to Epic H acceptable?~~ **Resolved
  (2026-07-03): yes.** This requirement covers the authorize/deny + raw-return decision only; logging
  that an unfiltered access *occurred* is a separate requirement in Epic H (0035, audit trail).
