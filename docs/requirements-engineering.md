# Requirements Engineering — A Working Primer

The professional method behind this repo's `requirements/` folder. Enough theory to write good
requirements and to recognize bad ones — kept lightweight on purpose. Standards referenced (not
reproduced): **ISO/IEC/IEEE 29148:2018** (requirements engineering) and **EARS** (Easy Approach to
Requirements Syntax, Mavin et al.).

## 1. What a requirement is
A requirement states **what** the system must do or how well — never **how** it's built. "The Logger
shall wrap private fields as pseudonyms" is a requirement; "use a `Dictionary<string,string>`" is a
design decision. Keep the two apart: requirements go in `requirements/`, design goes in the
requirement's *Design notes* section and in code.

### Types
- **Functional** — a behavior: input → output/effect. ("…shall round the timestamp to the minute.")
- **Quality / non-functional (NFR)** — a property or constraint on *how well*: performance,
  security, reliability, usability. ("…shall reject requests over 1,000,000 characters.")
- **Constraint** — an externally imposed limit: platform, standard, regulation. ("Target `net10.0`.")

## 2. Write it in EARS
EARS gives requirements a testable shape by forcing a trigger and a response. Five patterns —
pick the one that fits:

| Pattern | Template | Use for |
|---------|----------|---------|
| **Ubiquitous** | The `<system>` shall `<response>`. | always-true behavior |
| **Event** | **When** `<trigger>`, the `<system>` shall `<response>`. | response to an event |
| **State** | **While** `<state>`, the `<system>` shall `<response>`. | behavior during a mode |
| **Option** | **Where** `<feature>`, the `<system>` shall `<response>`. | behavior only if a feature/config is present |
| **Unwanted** | **If** `<condition>`, **then** the `<system>` shall `<response>`. | error / edge / abuse handling |

One requirement = **one** `shall`. If you write "and," you probably have two requirements — split
them (this is 29148's *singular* characteristic, and it's what makes each map to one test).

In this repo EARS is **encouraged, not mandated** — reach for it whenever a requirement has a clear
trigger; fall back to plain prose only when EARS would be forced.

## 3. Make it good — the 29148-lite checklist
Before a requirement leaves `Draft`, check each item. A requirement is:

- **Necessary** — remove it and something is lost. No gold-plating.
- **Singular** — one requirement, one `shall`.
- **Unambiguous** — one reading only. Kill "fast," "user-friendly," "etc." — quantify instead.
- **Complete** — states the trigger, the response, and the boundaries; needs no missing context.
- **Feasible** — buildable within our scope and tools.
- **Verifiable** — you can prove pass/fail. If you can't write a test/check for it, rewrite it.
- **Consistent** — doesn't contradict another requirement.
- **Traceable** — has a stable ID, points up to its source and down to what satisfies it.

A *set* of requirements additionally aims to be complete (no gaps), consistent (no conflicts), and
free of duplication.

## 4. Prioritize with MoSCoW
- **Must** — release is pointless without it.
- **Should** — important but has a workaround; can slip.
- **Could** — nice to have; first to drop.
- **Won't (this time)** — explicitly out, recorded so it isn't silently reintroduced.

## 5. Verification method
Every requirement declares *how* it will be proven — 29148's four methods:
- **Test** — execute code and observe (our default; xUnit).
- **Demonstration** — run it and watch the behavior, no measurement.
- **Inspection** — read the code/artifact.
- **Analysis** — reason/model/calculate (e.g. a headroom formula).

## 6. Traceability
The chain that lets you answer "why does this code exist?" and "is this requirement built?":

```
Source (DSS §x, a standard, a stakeholder)  →  Requirement (REQ-000N)  →  Test / code (satisfied_by)
```

In our template the `source` and `satisfied_by` frontmatter fields carry this both ways. Cite the
`iso24772` code and `stride` category when a requirement mitigates a known vulnerability/threat, so
security rationale is traceable too.

## 7. Acceptance criteria vs the requirement
- The **Requirement** is the single normative statement (the contract).
- **Acceptance criteria** are the concrete, checkable conditions that prove it — each phrased so
  exactly one test can pass or fail against it. Criteria are the bridge from requirement to xUnit.

## 8. How this maps to our files
Each file in `requirements/` is **one singular requirement** with YAML frontmatter (machine-readable
attributes) and body sections tagged **U** (user-facing → feeds the generated user guide) or **I**
(internal → rationale, design, tests). See [../requirements/TEMPLATE.md](../requirements/TEMPLATE.md)
for the exact shape and [workflow.md](workflow.md) for where writing requirements sits in the loop.

## Further reading (external, non-normative)
- Mavin et al., *EARS: The Easy Approach to Requirements Syntax* — the five patterns.
- ISO/IEC/IEEE 29148:2018 — the RE process and requirement quality characteristics.
- Robertson & Robertson, *Volere* — the "fit criterion" idea (measurable acceptance).
