# ISO/IEC 24772-1:2024 — Applicability to Managed C#

The full catalogue (Annex A) filtered for what actually bites **managed** C# on the CLR.
Source: my licensed copy of ISO/IEC 24772-1:2024. Codes and subclauses (`6.x` = language,
`7.x` = application) are from Table A.1.

**The lens:** the CLR is memory-safe by default — bounds-checked arrays, GC, no raw pointers.
That removes a class of C-style hazards *unless* you opt into `unsafe`, P/Invoke, `Span<T>`/
`stackalloc`, or reflection. Everything below is sorted by how much that safety net helps.

Legend: ✅ direct · ⚠️ conditional (unsafe/interop/reflection) · 🛡️ CLR-mitigated · 🌐 app-level.

---

## Tier 1 — Directly applicable (write guardrails for these)
Ordinary, safe C# can exhibit all of these. This is the working set for this repo.

### Types & numbers
| Code | Subcl. | Vulnerability | C# note |
|------|--------|---------------|---------|
| [IHN] | 6.2  | Type system | Implicit conversions, boxing/unboxing, `object`/`dynamic` erasing type safety. |
| [PLF] | 6.4  | Floating-point arithmetic | Never `==` on `double`/`float`; watch accumulation, `NaN`, use `decimal` for money. |
| [CCB] | 6.5  | Enumerator issues | An `enum` holds *any* backing int — `(Season)99` is legal; validate with `Enum.IsDefined`, handle `[Flags]` carefully. |
| [FLC] | 6.6  | Conversion errors | Narrowing casts truncate/wrap; prefer `checked`, `TryParse`, explicit widening. |
| [FIF] | 6.15 | Arithmetic wrap-around | Integer overflow **silently wraps** in the default `unchecked` context; use `checked` where it matters. |
| [PIK] | 6.16 | Shift for multiply/divide | Don't hand-optimize `* / %` into shifts; wrong for negatives, hurts clarity. |

### Names, declarations, expressions
| Code | Subcl. | Vulnerability | C# note |
|------|--------|---------------|---------|
| [NAI] | 6.17 | Choice of clear names | Already a house rule; intention-revealing names. |
| [WXQ] | 6.53 | Dead store | Value assigned then overwritten before use — a sign of a logic slip. |
| [YZS] | 6.65 | Unused variable | Compiler warns; treat as an error signal, don't silence. |
| [YOW] | 6.64 | Identifier name reuse | Shadowing a field/outer local with a same-named inner one. |
| [BJL] | 6.21 | Namespace issues | Ambiguous `using`, type name collisions across namespaces. |
| [LAV] | 6.25 | Missing initialization | Definite-assignment catches locals, but fields default to `0`/`null` silently — don't rely on it as intent. |
| [UJO] | 6.51 | Modifying constants | `readonly` on a mutable reference still lets the *object* mutate; `const` vs `static readonly` semantics. |
| [JCW] | 6.23 | Operator precedence | Parenthesize mixed `&& \|\| ?? ?:` and bitwise expressions. |
| [SAM] | 6.49 | Side-effects & order of evaluation | Avoid `a[i] = i++`-style expressions even though C# fixes left-to-right order. |
| [KOA] | 6.25 | Likely incorrect expression | Assignment-in-condition, empty `;` bodies, `=` where `==` meant. |
| [XYQ] | 6.62→ | Dead and deactivated code | `#if false`, unreachable branches — remove, don't comment out. |

### Control flow
| Code | Subcl. | Vulnerability | C# note |
|------|--------|---------------|---------|
| [CLL] | 6.27 | Switch & lack of static analysis | Handle every case; add a `default` that throws; enable exhaustiveness on `switch` expressions. |
| [EOJ] | 6.28 | Non-demarcation of control flow | Always brace `if`/`for`/`while` (enforced in `.editorconfig`). |
| [TEX] | 6.55 | Loop control variable abuse | Don't mutate the loop variable inside the body; beware closures capturing it. |
| [XZH] | 6.58 | Off-by-one error | Boundary `<` vs `<=`, `Length` vs `Length-1`. |
| [EWD] | 6.31 | Unstructured programming | No `goto`; single-purpose methods with clear exits. |
| [CSJ] | 6.27 | Passing parameters/return values | `ref`/`out`/`in` aliasing; return copies or immutables to avoid callers mutating internals. |
| [GDL] | 6.35 | Recursion | Unbounded recursion → uncatchable `StackOverflowException`; bound depth or iterate. |
| [OYB] | 6.34 | Ignored error status & unhandled exceptions | **Never** swallow exceptions; don't ignore return codes / `TryX` results. High priority. |

### Objects & memory
| Code | Subcl. | Vulnerability | C# note |
|------|--------|---------------|---------|
| [YAN] | 6.61 | Deep vs shallow copy | Reference members shared after `MemberwiseClone` / `with`; decide copy depth deliberately. |
| [XYL] | 6.59 | Memory leaks | GC ≠ leak-proof: un-unsubscribed events, static refs, undisposed `IDisposable`, captured closures. |
| [SYM] | 6.54 | Templates & generics | Constraints, variance (`in`/`out`), unexpected boxing of value-type generics. |
| [RIP] | 6.43 | Inheritance | Fragile base class; prefer composition; seal by default. |
| [BLP] | 6.44 | Liskov / contract violations | Subtypes must honor base pre/post-conditions — core SOLID topic here. |
| [PPH] | 6.45 | Redispatching | Calling a `virtual` method from a constructor runs the override before the subclass is built. |
| [BKK] | 6.22 | Polymorphic variables | Downcasts (`as`/`is`) that assume a runtime type; pattern-match instead. |
| [XYH] | 6.57 | Null pointer dereference | The classic `NullReferenceException`; **enable nullable reference types** and treat their warnings as errors. Top of the list. |

### Libraries & concurrency
| Code | Subcl. | Vulnerability | C# note |
|------|--------|---------------|---------|
| [TRJ] | — | Argument passing to library functions | Validate args at your boundary; don't assume a library re-checks. |
| [HJW] | 6.50 | Unanticipated exceptions from libraries | Know what a BCL/3rd-party call can throw; catch narrowly. |
| [MEM] | 6.57 | Deprecated language features | Honor `[Obsolete]`; don't reach for legacy APIs. |
| [BRS] | 6.56 | Obscure language features | Prefer readable idioms over clever ones. |
| [MXB] | — | Suppression of run-time checking | `#nullable disable`, `unchecked`, `unsafe` remove safety — justify every use. |
| [CGA] | 6.59 | Concurrency – activation | Correct `Task`/thread startup; don't fire-and-forget `async void`. |
| [CGT] | 6.62 | Concurrency – directed termination | Use `CancellationToken`, not deprecated `Thread.Abort`. |
| [CGS] | 6.60 | Concurrency – premature termination | Handle tasks that die mid-work; observe faulted `Task`s. |
| [CGX] | 6.61 | Concurrent data access | Shared mutable state → races; lock, or use immutable/concurrent types. |
| [CGM] | 6.63 | Lock protocol errors | Consistent lock ordering to avoid deadlock; don't `await` inside a `lock`. |

---

## Tier 2 — Conditional: only via `unsafe`, interop, or reflection
Not reachable from ordinary safe C#. Relevant only if a requirement explicitly opts in.

| Code | Vulnerability | Triggered by |
|------|---------------|--------------|
| [HCB] | Buffer boundary violation | `unsafe`, `stackalloc`, `Span<T>`/`Marshal` misuse |
| [HFC] | Pointer type conversions | `unsafe` pointers |
| [RVG] | Pointer arithmetic | `unsafe` pointers |
| [DCM] | Dangling refs to stack frames | `ref` returns / `ref` locals / `stackalloc` escaping |
| [AMV] | Type-breaking reinterpretation | `Unsafe.As`, `MemoryMarshal`, explicit `StructLayout` unions |
| [CJM] | String termination | Only when marshalling to native null-terminated strings |
| [STR] | Bit representations | Interop / binary serialization / endianness |
| [DJS] | Inter-language calling | P/Invoke, COM interop |
| [NSQ] | Library signature | Wrong P/Invoke signature vs native ABI |
| [NYY] | Dynamically-linked / self-modifying code | `Assembly.Load`, `Reflection.Emit`, plugins |
| [SKL] | Inherently unsafe operations | `unsafe`, `Marshal`, `System.Runtime.CompilerServices.Unsafe` |
| [EWF] | Undefined behaviour | Practically only via `unsafe` or unsynchronized data races |
| [OTR] | Subprogram signature mismatch | Delegates / reflection binding (compiler catches direct calls) |
| [NMP] | Pre-processor directives | C# has no macros; only `#if`/`#define` conditional compilation |
| [LRM] | Extra intrinsics | Vendor/runtime intrinsics (`System.Runtime.Intrinsics`) |

---

## Tier 3 — Mitigated by the CLR (know them, rarely a guardrail)
The runtime converts what would be C memory corruption into a **caught, deterministic
exception**. The logic bug still exists — it just fails safely.

| Code | Vulnerability | Why it's softened |
|------|---------------|-------------------|
| [XYZ] | Unchecked array indexing | Bounds-checked → `IndexOutOfRangeException`, not corruption |
| [XYW] | Unchecked array copying | `Array.Copy`/`Buffer.BlockCopy` validate lengths |
| [XYK] | Dangling reference to heap | GC keeps objects alive while referenced |
| [BQF] | Unspecified behaviour | CLR defines far more than C; some ordering/timing remains |
| [FAB] | Implementation-defined behaviour | Runtime/platform/culture differences (e.g. `string` sort) |

---

## Tier 4 — Application-level (Annex A.3): applies by domain, not by language
Independent of C#. Pull the relevant rows into a requirement **when the code does that thing**.
For a pure algorithm-practice exercise, most are out of scope.

| Code | Subcl. | Vulnerability | Applies when you… |
|------|--------|---------------|-------------------|
| [REU] | 7.31 | Fault tolerance / failure strategies | design how the program fails (relevant broadly) |
| [KLK] | 7.32 | Distinguished values in data types | use sentinels/magic values instead of `null`/`Option` |
| [RST] | 6.12 | Injection | build SQL/commands/queries from input |
| [XYT] | — | Cross-site scripting | render web output |
| [EWR] | 7.11 | Path traversal | open files from user-supplied paths |
| [EFS] | 7.6 | Unchecked/tainted input | trust data from outside |
| [XZP] | — | Resource exhaustion | allocate based on input size |
| [XYN] | — | Adherence to least privilege | run with more rights than needed |
| [XYO] | — | Privilege sandbox issues | host untrusted code |
| [XYS] | — | Executing/loading untrusted code | load plugins/assemblies |
| [XZK] | — | Sensitive info not cleared | hold secrets/keys in memory |
| Auth/crypto family | 7.x | [XYP] hard-coded creds, [XYM] protected creds, [XZN] access control, [XZO] auth logic, [XZR] signatures, [XZS] crypto step, [MVX] unsalted hash, [DLB] integrity check, [BJE] authorization, [PYQ] open redirect, [WPL] auth attempt limits, [DHU] untrusted control sphere | build authentication, authorization, or crypto |
| Timing family | 7.x | [CCM] time measurement, [CCI] clock issues, [CDJ] drift/jitter, [CGY] secure shared comms | depend on time or share resources across processes |
| Input family | 7.x | [CBF] file upload, [HTS] resource names, [XZQ] unquoted path, [XZL] discrepancy leak | accept files/named resources |
| [BVQ] | 7.2 | Unspecified functionality | ship behavior not in the spec |

---

## How to use this
- **Tier 1** is the candidate list for concrete rules in [guardrails.md](guardrails.md) and for
  review checklists. Start there.
- When a requirement opts into `unsafe`/interop, re-read the relevant **Tier 2** rows first.
- Cite the code (e.g. `[XYH]`) in commit messages or the learning log when a change addresses one —
  it ties the practice back to the standard.
