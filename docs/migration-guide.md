# Migration Guide: LeetKeen.Results v1 → v2

This document lists all breaking changes introduced in v2. Use it to audit and refactor code
that depends on the previous version of the library.

---

## IError / Error

### Renamed members

| v1 | v2 | Action |
| -- | -- | ------ |
| `IError.Message` | `IError.Text` | Rename all usages. `Text` is now `string?` (nullable). |
| `IError.Reasons` | `IError.Children` | Rename all usages. |

### Removed members

| v1 | Replacement in v2 | Action |
| -- | ----------------- | ------ |
| `IError.IsSecret` | `DataItem.Visibility` | Replace `IsSecret = true` with `Visibility != 0` on the specific DataItem(s) that carry sensitive data. See **DataBag / DataItem** below. |
| `IError.Data` (`Dictionary<string, object>`) | `IError.Payload` (`DataBag`) | See **DataBag / DataItem** below. |

### New members (no action required, but worth knowing)

| Member | Description |
| ------ | ----------- |
| `IError.IsTransient` | `true` = transient failure; the caller/UI may offer a Retry. Default `false`. |
| `IError.CorrelationId` | Computed `string?`; reads `Payload["correlationId"]` with `Visibility = 0`. |

---

## DataBag / DataItem (replaces `Dictionary<string, object>`)

`IError.Data` was a `Dictionary<string, object>`. It is replaced by `IError.Payload: DataBag`.

```
DataBag
  Items: IReadOnlyList<DataItem>

DataItem
  Key:        string
  Value:      object
  Visibility: int   // 0 = public; non-zero = private (stripped at trust boundaries)
```

### Migration steps

1. **Reading a value:**

   ```csharp
   // v1
   error.Data["entityId"]

   // v2
   error.Payload.Items.FirstOrDefault(i => i.Key == "entityId")?.Value
   ```

2. **Writing a value (fluent builder):**

   ```csharp
   // v1
   error.WithData("entityId", id)

   // v2 — public item
   error.WithData("entityId", id)          // Visibility defaults to 0

   // v2 — private/sensitive item
   error.WithData("internalDetail", value, visibility: 1)
   ```

3. **`IsSecret = true` pattern:**

   ```csharp
   // v1
   new Error { IsSecret = true, Data = { ["token"] = rawToken } }

   // v2 — mark the specific item as private
   new Error().WithData("token", rawToken, visibility: 1)
   ```

   Choose any non-zero `Visibility` value. The library strips all `Visibility != 0` items at
   trust boundaries. Your system decides what specific values mean; document them as constants.

---

## ErrorExtensions

| v1 | v2 | Action |
| -- | -- | ------ |
| `.WithReason(error)` | `.WithChild(error)` | Rename all usages. |
| `.WithData(key, value)` | `.WithData(key, value, visibility = 0)` | Compatible as-is (default visibility = 0). Add `visibility` argument where sensitivity matters. |

---

## Warning / IWarning (new type)

`Warning` is a new type in v2. No v1 equivalent exists; no migration required.

```
IWarning
  Code:     int
  Text:     string?
  Payload:  DataBag
  Children: IReadOnlyList<IWarning>
```

A warning with `Children.Any() == true` acts as an aggregate container (no own message rendered;
UI shows it as an expandable group). A warning with no children is a standalone warning with its
own message.

---

## IResult / Result\<T\>

### New members

| Member | Description |
| ------ | ----------- |
| `IResult.Warnings` | `IReadOnlyList<IWarning>`. Empty by default on existing results. |

`IsSuccess()` is unchanged: `!Errors.Any()`. Warnings do not affect success/failure.

---

## StandardErrors factories

All factory methods now return errors with `DataBag`-based `Payload` instead of a `Dictionary`.
Method signatures are unchanged; update any code that reads `.Data[key]` directly (see
**DataBag / DataItem** migration above).

---

## Trust boundary stripping

New utility to strip all private data before sending a result across a trust boundary
(e.g., internal API → external client):

```csharp
// v2 — strip all DataItems with Visibility != 0 from the entire Result tree
var publicResult = result.StripPrivateData();
```

- Strips recursively through `Error.Children` and `Warning.Children`.
- `CorrelationId` (Visibility = 0) is always preserved.
- The original `result` is not mutated; a new instance is returned.
- Log the original (before stripping) if you need the full internal context.
