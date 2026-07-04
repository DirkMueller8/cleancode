---
id: REQ-0010
slug: digest-based-no-raw-retained
title: Derive pseudonym mappings from a salted digest, retaining no raw value
epic: Filtering Engine
status: Done
priority: Must
scope: now
verification: test
source: ["DSS §8"]
satisfied_by: ["tests/Logger.Core.Tests/PseudonymContextTests.cs"]
concepts: [DIP]
stride: [InformationDisclosure]
iso24772: [MVX]
user_facing: false
doc_chapter: "Filtering & pseudonyms"
created: 2026-07-03
updated: 2026-07-03
---

## Summary
As a privacy owner, I want the pseudonym map to hold only a salted hash of each value, so that
compromising the filtered-view state does not leak the private values it stands for.

## Requirement
The Logger shall key each pseudonym mapping by a salted secure digest of the raw value, and shall not
retain the raw value in the pseudonym context.

## Acceptance criteria
- [x] After filtering, the context contains no raw value (verified behaviourally — see note).
- [x] Equal values produce equal digests → equal pseudonyms (supports REQ-0009).
- [x] The digest is salted per context, so identical values in different contexts need not share a digest.
- [x] The hashing algorithm is injected, not hard-coded.

## Design notes
`IHasher { string Digest(string value, string salt); }` — DIP seam; default a SHA-256 implementation.
The salt is per-context (generated when the context is created). The `[MVX]` guardrail is the reason
salting is explicit rather than a bare hash.

**As built (2026-07-04):** `PseudonymContext` stores only `digest → number` maps (the raw value is
passed to the hasher and then discarded). "Contains no raw value" is verified *behaviourally* rather
than by reflecting on internals: a test injects a **colliding hasher** so two different raw values map
to the same number — which is only possible if the digest, not the raw value, is the key. Exposing
internal state purely to assert on it would be worse than this black-box proof. The per-context salt
comes from the constructor (random in production, fixed in tests), keeping the context deterministic.

## Security & traceability
- **Why / rationale:** DSS §8 stores "a secure digest of the unfiltered data value" so raw private
  data is never held in the mapping. Salting defeats precomputed-hash (rainbow-table) inference.
- **Source:** DSS §8
- **Threat mitigated (STRIDE):** InformationDisclosure  ·  **ISO 24772:** [MVX] one-way hash without a salt

## Open questions
- ~~Per-context salt, or also per-field?~~ **Resolved (2026-07-03): per-context.** One salt per
  (user, log) context. Identical values in *different* contexts already get different digests; per-field
  salting is deferred as a possible later hardening, not built now.
