# Logger — User Guide

_Generated from the requirements by DocGen. Do not edit by hand._

## Defining what you log

### Define a log schema of typed fields

As an application author, I want to declare the shape of my log events up front, so that Logger knows which fields exist and how each should be handled.

The Logger shall represent a schema as a set of named LogTypes, where each LogType declares a set of named, typed fields.

```
Schema:
  LogType "login"  -> fields: timestamp:Time, ipaddr:IpAddress, user:String, password:String
  LogType "logout" -> fields: timestamp:Time, ipaddr:IpAddress, user:String
```

### Require a timestamp and at least one other identifying field

As an auditor, I want every log event to carry a timestamp and at least one identifying field, so that records are attributable and form a reliable audit trail.

If a LogType does not declare a timestamp field and at least one additional identifying field, then the Logger shall reject the schema.

```
Rejected:  LogType "ping" -> fields: timestamp:Time (no other identifying field)
Accepted:  LogType "ping" -> fields: timestamp:Time, ipaddr:IpAddress
```

### Every field must declare a disposition

As a privacy owner, I want to be forced to declare the sensitivity of every field, so that no data item is logged without a deliberate decision about how it may be exposed.

The Logger shall require every field in a schema to declare a disposition (one of `nonsensitive`, `private`, or a named filter), and shall reject any schema with a field whose disposition is missing.

```
Rejected:  user:String                    (no disposition)
Accepted:  user:String  disposition=private
           ipaddr:IpAddress disposition=country
           http:String  disposition=nonsensitive
```

## Recording events

### Accept an event only if it matches its LogType schema

As an application author, I want Logger to reject events that don't match my declared schema, so that stored logs stay well-formed and trustworthy.

When an event is submitted for a LogType, the Logger shall accept it only if the event's fields match the declared field names and types of that LogType.

```
Schema "login": timestamp:Time, ipaddr:IpAddress, user:String
Accepted:  {timestamp: 1234567890, ipaddr: "12.34.56.78", user: "SAM"}
Rejected:  {timestamp: 1234567890, ipaddr: "not-an-ip", user: "SAM"}   (ipaddr wrong type)
Rejected:  {timestamp: 1234567890, user: "SAM"}                        (missing ipaddr)
Rejected:  {timestamp: 1234567890, ipaddr: "12.34.56.78", user: "SAM", extra: "x"}  (unknown field)
```

### Enforce request and field size limits

As an operator, I want oversized inputs rejected, so that a single request cannot exhaust resources or smuggle in unbounded data.

If a request exceeds 1,000,000 characters, or any individual field value exceeds 10,000 characters, then the Logger shall reject it.

```
Accepted:  user value of 12 chars
Rejected:  user value of 10,001 chars          (field limit)
Rejected:  total request of 1,000,001 chars    (request limit)
```

## Filtering & pseudonyms

### Copy nonsensitive fields unchanged into the filtered view

As an operator, I want nonsensitive fields to appear exactly as logged in the filtered view, so that routine monitoring keeps full access to the data that carries no privacy risk.

Where a field's disposition is `nonsensitive`, the Logger shall copy its value unchanged into the filtered view.

```
Raw:      http: "POST", url: "login.html"
Filtered: http: "POST", url: "login.html"
```

### Wrap private fields as pseudonyms with a format/length hint

As an operator, I want private fields shown as pseudonyms with a small format hint, so that I can
work with the logs without ever seeing the raw private value.

Where a field's disposition is `private`, the Logger shall replace its value in the filtered view with
a pseudonym composed of a type prefix, a per-context sequence number, and a format/length hint.

```
Raw:      user: "SAM",  password: ">1<}2{]3[\4/"
Filtered: user: USER1(3), password: PW1(12)
```
`USER1(3)` = username field, first distinct value in this context, 3 characters long.

### Map equal values to equal pseudonyms within a context

As an operator, I want the same hidden value to always show the same pseudonym, so that I can
correlate related events (e.g. "all requests from IP `US7`") without seeing the real value.

While within one pseudonym context, the Logger shall assign equal raw values the same pseudonym and
distinct raw values distinct pseudonyms.

```
Within one context:
  events with IP 66.77.88.99  -> all shown as US1(v4)
  event   with IP 10.0.0.1    -> shown as US2(v4)
Correlation: querying US1 finds every event from 66.77.88.99 without revealing it.
```

### Round timestamps to the minute in the filtered view

As an operator, I want timestamps coarsened to the minute in filtered views, so that precise timing
can't be used to single out or correlate individuals unnecessarily.

Where a field's disposition is `minute`, the Logger shall round its timestamp down to the start of the
containing minute in the filtered view.

```
Raw:      2022/10/19 08:09:10
Filtered: 2022/10/19 08:09
```

### Map IP addresses to country of origin in the filtered view

As an operator, I want IP addresses reduced to their country in filtered views, so that I can reason
about geography (and still correlate) without exposing individual addresses.

Where a field's disposition is `country`, the Logger shall replace the IP address with a pseudonym
combining its country and a per-context sequence number, plus an address-family hint.

```
Raw:      66.77.88.99
Filtered: US1(v4)          (first distinct US address in this context, IPv4)
```

### Support pluggable custom filters without modifying existing code

As an application author, I want to add my own field filter for a data type Logger doesn't ship, so
that I can classify custom data without changing Logger's code.

Where an application registers a custom filter under a disposition name, the Logger shall apply that
filter to fields declaring that disposition, without any modification to existing filter classes or
the view assembler.

```
Register:  "postcode" -> PostcodeFilter   (a new class the app supplies)
Schema:    zip:String disposition=postcode
Filtered:  zip -> ZIP1(5)                  (behavior defined by PostcodeFilter)
```

### Assemble a full filtered view of an event

As an operator, I want to see a whole event in filtered form, so that each field is shown according to
its declared disposition in a single coherent record.

When an event is filtered, the Logger shall produce a filtered view in which each field has been
transformed by the filter registered for that field's disposition.

```
Schema "login": timestamp:minute, ipaddr:country, http:0, url:0, user:private, password:private
Raw:      2022/10/19 08:09:10  66.77.88.99  POST  login.html  {user: "SAM", password: ">1<}2{]3[\4/"}
Filtered: 2022/10/19 08:09     US1(v4)      POST  login.html  {user: USER1(3), password: PW1(12)}
```

### Provide the unfiltered (raw) view to authorized callers

As an authorized investigator, I want to retrieve the raw event when strictly necessary, so that I can diagnose incidents that the filtered view can't resolve.

Where a caller is authorized for unfiltered access, the Logger shall return the raw event values; if the caller is not authorized, then the Logger shall deny the request.

```
Authorized:   returns {user: "SAM", password: ">1<}2{]3[\4/"}
Unauthorized: request denied (no raw values returned)
```

### Maintain pseudonym mappings separately per (user, log) context

As a privacy owner, I want each viewer's pseudonym mappings kept separate, so that one user's view of
the logs never leaks correlations into another's, and a view can be reset independently.

The Logger shall maintain pseudonym mappings separately per (user, log) context, so that allocations
made in one context do not affect any other context.

```
Context A:  SAM -> USER1,  BOB -> USER2
Context B:  BOB -> USER1            (independent — B has its own counters and salt)
```
