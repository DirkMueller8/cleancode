---
id: REQ-0051
slug: runnable-demo
title: A runnable end-to-end demo of the Logger
epic: Tooling
status: Done
priority: Could
scope: now
verification: demo
source: []
satisfied_by: ["samples/LoggerDemo/Program.cs"]
concepts: [CompositionRoot, DIP]
stride: []
iso24772: []
user_facing: false
doc_chapter: "Samples"
created: 2026-07-04
updated: 2026-07-04
---

## Summary
As someone evaluating the Logger, I want a single command that runs the whole flow, so that I can see
raw data become a filtered view (and the security rules fire) without reading tests.

## Requirement
The demo shall wire the Logger from its public API (the composition root) and narrate one end-to-end
run: a session records events, they are shown as a filtered view, correlation queries run, the
inference guard and unfiltered-access rules fire, and the session closes.

## Worked example
```
dotnet run --project samples/LoggerDemo
  -> raw:      user=SAM password=hunter2pass!
  -> filtered: user=USER1(3) password=PW1(12)
  -> [ipaddr = US1] -> N events ;  [ipaddr = 66.77.88.99] -> REJECTED
  -> investigator -> GRANTED (raw) ;  guest -> DENIED
```

## Acceptance criteria
- [x] `dotnet run --project samples/LoggerDemo` runs to completion and prints the raw vs filtered view.
- [x] The demo shows a correlation query matching by symbol, and a raw-value query being rejected.
- [x] The demo shows unfiltered access granted to an authorized user and denied to another.
- [x] The demo shows the session lifecycle (Hello → … → Goodbye invalidating the token).

## Design notes
`samples/LoggerDemo` is a console app — the composition root the library architecture points at. It
references `Logger.Services` (and `Logger.Core` transitively) and supplies two demo-only stubs
(`IGeoLookup` → "US", an allow-list `IUnfilteredAccessPolicy`), since real geo/auth are out of scope.
Verified by **running it** (verification: demo), not unit tests — the underlying logic is already
covered by 115 tests; this is glue/narration.

## Security & traceability
- **Why / rationale:** Makes the system tangible; exercises the public API as a real caller would.
- **Source:** —
- **Threat mitigated (STRIDE):** —  ·  **ISO 24772:** —

## Open questions
- None.
