# Glossary — Controlled Vocabulary

The agreed meaning of each term used in the Logger requirements. A shared vocabulary is a core RE
practice: it removes ambiguity and keeps every requirement, test, and doc using words the same way.
Terms trace to the source design doc (**DSS** = *Designing Secure Software*, Kohnfelder — gitignored).

| Term | Meaning |
|------|---------|
| **Logger** | The overall privacy-aware logging component being specified. |
| **Recorder** | The service that receives log events and writes them to storage (DSS §4). In this repo, an in-memory stub. |
| **Viewer** | The (web) front end for inspecting logs. Out of scope here except as a domain concept. |
| **Event** | One recorded occurrence: a timestamp plus schema-defined fields (DSS §5, §6.3). |
| **Log** | An append-only sequence of events for one application/log type. |
| **Schema** | The declaration of a log's `LogType`s and, per type, the named/typed fields and their dispositions (DSS §5). |
| **LogType** | A named category of event (e.g. `login`, `logout`) with its own field set. |
| **Field** | A named, typed data item within an event (e.g. `user`, `ipaddr`). |
| **Disposition** | The per-field rule for how the field appears in a filtered view: `nonsensitive` (`0`), `private`, or a named filter (`minute`, `country`, custom). Every field must declare one. |
| **Filter** | A component that transforms a field value for the filtered view according to its disposition. Pluggable (Strategy/OCP). |
| **Filtered view** | A projection of an event in which sensitive fields are wrapped, safe for routine inspection (DSS §2.3). |
| **Unfiltered view** | The raw event including private values; available only with authorization. |
| **Wrap** | To replace a private value with a pseudonym in the filtered view. |
| **Pseudonym** | A stable symbolic identifier substituted for a private value, carrying a type prefix and a format/length hint (e.g. `USER1(3)`, `PW1(12)`, `US1(v4)`). |
| **Format/length hint** | The parenthetical suffix on a pseudonym revealing shape without content (`(3)` = 3 chars, `(v4)` = IPv4). |
| **Pseudonym context** | The `(user, log)` scope within which pseudonym assignments are stable and consistent (DSS §8). Cleared manually or after 24h idle. |
| **Digest** | A secure hash of a value used as the mapping key, so raw private data need not be stored (DSS §8). |
| **Session** | A connected logging conversation: `Hello → (Schema \| Event)* → Goodbye` (DSS §6). |
| **Token** | The opaque session identifier issued on `Hello`, invalidated on `Goodbye` (DSS §6.1). |
| **Retention** | The configured period after which an event is expired and securely deleted (DSS §9). |
| **DSS** | *Designing Secure Software* (Kohnfelder) — the source design doc; cited by section number only. |

> Add a term here the first time a requirement needs it. If two requirements would use a word
> differently, that's a conflict to resolve — fix the glossary first, then the requirements.
