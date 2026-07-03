# Requirements

Each feature starts life here as a spec before any code is written. This is step 1 of the
[workflow](../docs/workflow.md).

## How it works
- Copy [TEMPLATE.md](TEMPLATE.md) to `NNNN-short-title.md`, using the next free 4-digit number.
- Fill in intent, scope, and **acceptance criteria**. Claude can help draft it — just ask.
- Statuses: `Draft` → `In progress` → `Done` (or `Parked`).

## Conventions
- Numbers are sequential and never reused, even if a requirement is parked or dropped.
- Titles are short and kebab-cased.
- Acceptance criteria are the contract: the work isn't `Done` until each one is demonstrably met.

## Index
| #    | Title                              | Status | Concept(s)                     |
|------|------------------------------------|--------|--------------------------------|
| 0001 | Example — FizzBuzz via rule set    | Draft  | OCP, DIP, Strategy (reference) |
