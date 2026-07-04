# Requirements

Each feature starts life here as a spec before any code is written — step 1 of the
[workflow](../docs/workflow.md). The method is summarized in
[docs/requirements-engineering.md](../docs/requirements-engineering.md).

## Conventions
- **One file = one *singular* requirement** (`NNNN-short-title.md`), copied from
  [TEMPLATE.md](TEMPLATE.md). Epics/features are tracked only in the index below.
- Numbers are sequential and never reused, even if a requirement is parked or dropped.
- Frontmatter carries machine-readable attributes (priority, scope, traceability, security tags);
  body sections are tagged **(U)** user-facing or **(I)** internal for user-doc generation.
- Statuses: `Draft` → `In progress` → `Done` (or `Parked`).
- Shared terms live in [glossary.md](glossary.md). Source doc: **DSS** = *Designing Secure Software*
  (Kohnfelder), gitignored; cited by section number only.
- `scope`: `now` (near-term build path) · `later` (in scope, not yet) · `out-of-scope-infra`
  (recorded for traceability; not built here).

## Priority (MoSCoW) & scope at a glance
The Logger domain, decomposed. Authored files are linked; backlog rows become files when scheduled.

### Epic A — Schema & Validation · DSS §5, §6.3  ✅ **complete**
| #    | Title | Priority | Scope | Status |
|------|-------|----------|-------|--------|
| [0002](0002-define-log-schema.md) | Define a log schema of typed fields | Must | now | Done ✅ |
| [0003](0003-require-timestamp-and-identifier.md) | Require timestamp + ≥1 identifying field | Must | now | Done ✅ |
| [0004](0004-disposition-mandatory-per-field.md) | Every field must declare a disposition | Must | now | Done ✅ |
| [0005](0005-event-conforms-to-schema.md) | Accept an event only if it matches its schema | Must | now | Done ✅ |
| [0006](0006-size-and-encoding-limits.md) | Enforce request & field size limits | Should | now | Done ✅ |

### Epic B — Filtering Engine · DSS §2.3, §5, §8  ✅ **complete** *(the SOLID/OCP showcase)*
| #    | Title | Priority | Scope | Status |
|------|-------|----------|-------|--------|
| [0007](0007-nonsensitive-passthrough.md) | Copy nonsensitive fields unchanged | Must | now | Done ✅ |
| [0008](0008-private-field-wrapping.md) | Wrap private fields as pseudonyms + hint | Must | now | Done ✅ |
| [0009](0009-pseudonym-stability-in-context.md) | Equal values → equal pseudonyms in a context | Must | now | Done ✅ |
| [0010](0010-digest-based-no-raw-retained.md) | Salted-digest mapping, retain no raw value | Must | now | Done ✅ |
| [0011](0011-minute-filter.md) | Round timestamps to the minute | Should | now | Done ✅ |
| [0012](0012-country-filter.md) | Map IP → country of origin | Should | now | Done ✅ |
| [0013](0013-pluggable-custom-filters.md) | Pluggable custom filters (OCP) | Must | now | Done ✅ |
| [0014](0014-assemble-filtered-view.md) | Assemble a full filtered view | Must | now | Done ✅ |
| [0015](0015-unfiltered-view.md) | Unfiltered view for authorized callers | Could | now | Done ✅ |

### Epic C — Pseudonym-context lifecycle · DSS §8  ✅ **complete**
| #    | Title | Priority | Scope | Status |
|------|-------|----------|-------|--------|
| [0016](0016-pseudonym-context.md) | Maintain mappings per (user, log) context | Must | now | Done ✅ |
| [0017](0017-clear-context.md) | Clear a context's mappings on request | Should | now | Done ✅ |
| [0018](0018-expire-context.md) | Auto-expire a context after 24h idle `[CCI]` | Should | now | Done ✅ |

### Epic D — Query over filtered logs · DSS §7
| #    | Title | Priority | Scope | Status |
|------|-------|----------|-------|--------|
| [0019](0019-query-by-symbol.md) | Query by symbolic identifier (e.g. `[IP = US1]`) | Could | now | Done ✅ |
| [0020](0020-forbid-exact-match-on-filtered-fields.md) | Forbid exact-value match on filtered fields (inference guard) | Should | now | Done ✅ |
| 0021 | Default ordering & paging | Could | later | Draft |

### Epic E — Session RPC (in-memory stub) · DSS §6  ✅ **complete**
| #    | Title | Priority | Scope | Status |
|------|-------|----------|-------|--------|
| [0022](0022-session-lifecycle.md) | Session lifecycle order (Hello → Schema/Event* → Goodbye) | Should | now | Done ✅ |
| [0023](0023-session-token.md) | Issue a ≥120-bit session token | Could | now | Done ✅ |
| [0024](0024-token-invalidation.md) | Invalidate the token on Goodbye | Should | now | Done ✅ |

Built in the `Logger.Services` project (in-memory service stubs; depends on `Logger.Core`).

### Epics F–H — Storage, retention, access *(mostly out-of-scope-infra)* · DSS §3, §5, §8, §9
| #    | Title | Scope |
|------|-------|-------|
| [0025](0025-append-only-store.md) | Append-only persistence (in-memory `ILogStore`) | now · Done ✅ |
| 0029 | Storage-headroom alert (<10h) — pure calc | later |
| 0031 | Retention-expiry evaluation (`IClock`) | later |
| 0033 | Access-list config model (filtered/unfiltered/approval) | later |
| 0035 | Audit trail of unfiltered access | later |
| 0026–0028, 0032, 0034, 0036–0037 | AES-at-rest, HSM, RAID, secure delete, approval web/email, SSO | out-of-scope-infra |

### Tooling *(0050+ series)*
| #    | Title | Scope | Status |
|------|-------|-------|--------|
| [0050](0050-user-guide-generator.md) | Requirements → user-guide generator (`tools/DocGen`) | now | Done ✅ |

Run it: `dotnet run --project tools/DocGen` → writes [docs/user/user-guide.md](../docs/user/user-guide.md).

| [0051](0051-runnable-demo.md) | Runnable end-to-end demo (`samples/LoggerDemo`) | now | Done ✅ |

Run it: `dotnet run --project samples/LoggerDemo` → narrated end-to-end run of the whole system.

## Legacy
- [0001-example-fizzbuzz.md](0001-example-fizzbuzz.md) — predates the current template; kept as a
  reference example only.
