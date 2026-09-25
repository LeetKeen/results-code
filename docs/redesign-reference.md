# Redesign Reference: Result Library per TODO.md

> Этот файл — мой рабочий конспект. Читать когда берёшь любую задачу из эпика "Result Library Redesign".

---

## Что меняется: текущий API → новый API

### Result<T>

| Сейчас | После redesign |
|--------|---------------|
| `Errors: IReadOnlyList<IError>` | без изменений |
| — | + `Warnings: IReadOnlyList<IWarning>` |
| `IsSuccess()` = `!Errors.Any()` | без изменений |

Ключевое: предупреждения могут быть у как успешного, так и провалившегося результата.

---

### IError / Error

| Поле сейчас | Поле после | Примечание |
|-------------|-----------|-----------|
| `Code: int` | `Code: int` | 0 = no translator expected |
| `Message: string` | `Text: string?` | nullable; auto-trim оставить |
| `Data: Dictionary<string,object>` | `Payload: DataBag` | см. DataBag ниже |
| `IsSecret: bool` | — (удаляется) | заменяется DataItem.Visibility |
| `Reasons: IReadOnlyList<IError>` | `Children: IReadOnlyList<IError>` | переименование |
| — | `IsTransient: bool` | новое поле |
| — | `CorrelationId: string?` | вычисляемое read-only свойство; см. ниже |

**CorrelationId** — computed read-only свойство, которое достаёт значение DataItem с `Key = "correlationId"` из `Payload`. Хранится в Payload, но поверхность доступа удобная:

```csharp
public string? CorrelationId =>
    Payload.Items.FirstOrDefault(i => i.Key == "correlationId")?.Value as string;
```

Visibility этого DataItem должен быть 0 (публичный) — иначе он будет удалён при stripping.

**IsTransient** — true = сбой временный (сеть, БД timeout), можно предложить Retry в UI.
Решение о retry принимает UI/caller на основе этого флага; логика retry не входит в библиотеку.

**Children** — иерархия ошибок. Если топ-уровень переведён транслятором — дочерние ошибки
пользователю не показываются (транслятор закрыл контекст). Если не переведён — ищем ниже.
Полная иерархия всегда уходит в structured logs (не задача библиотеки, задача caller'а).

---

### DataBag / DataItem (новые типы)

```
DataBag
  Items: DataItem[]

DataItem
  Key:        string
  Value:      object
  Visibility: int   // 0 = public; != 0 = private (strip at boundary)
```

**Visibility — int, не enum** (platform-agnostic). Библиотека работает только с 0 vs != 0.
Конкретные ненулевые значения — зона ответственности потребителя.

**Граница доверия (trust boundary crossing):**
1. Все DataItem с `Visibility != 0` удаляются рекурсивно (через Children).
2. До удаления — полное дерево пишется в structured logs (не задача библиотеки!).
3. CorrelationId: хранится как `DataItem { Key="correlationId", Visibility=0 }` — всегда публичный.

**Важно:** `Text` и `Code` всегда считаются публичными. Чувствительные данные нельзя класть в Text/Code — дисциплина команды, не библиотека.

---

### Warning / IWarning (новый тип)

```
Warning
  Code:        int
  Text:        string?
  Payload:     DataBag
  Children:    Warning[]
```

`IsContainer` **отсутствует** — режим определяется наличием Children:

| Children.Any() | Режим | Поведение UI |
| -------------- | ----- | ------------ |
| false | Standalone warning | Своё сообщение |
| true | Aggregate container | Объединяет дочерние; Text может быть заголовком группы или null |

**IsTransient у Warning отсутствует** — предупреждения не являются ошибками, retry не применим.

---

### Translators (вне библиотеки)

Библиотека только предоставляет контракт данных. Транслятор — внешний компонент:
- вход: `(Code, Payload)` + язык/контекст
- выход: user-facing text/structure
- если совпал топ-уровень — дочерние скрыты
- если не совпал — ищем ниже
- Code=0 → только Text, транслятор не ожидается

---

## Что НЕ входит в библиотеку

- Retry-логика (в caller/UI, на основе IsTransient)
- Строки локализации (в реализациях транслятора)
- Семантика конкретных non-zero Visibility (в потребляющей системе)
- Логирование (в инфраструктуре)

---

## Текущая кодовая база: ключевые файлы

```
src/LeetKeen.Results/
  IResult.cs / IResult{T}.cs / Result{T}.cs   — core types
  IError.cs / Error.cs                          — error model (МЕНЯЕТСЯ)
  ErrorExtensions.cs                            — fluent builders (МЕНЯЕТСЯ)
  ResultExtensions.cs                           — sync pipeline ops
  ResultExtensions.Async.cs                     — async pipeline ops
  StandardErrorCodes.cs                         — integer constants
  StandardErrors.cs                             — factory methods
  StandardDataKeys.cs                           — dictionary key constants

src/Tests/LeetKeen.Results.UnitTests/           — MSTest + Shouldly
```

---

## Порядок реализации (рекомендованный)

1. `DataBag` / `DataItem` (нет зависимостей)
2. Redesign `IError` / `Error` (зависит от DataBag)
3. `IWarning` / `Warning` (зависит от DataBag)
4. Добавить `Warnings` в `IResult<T>` / `Result<T>` (зависит от Warning)
5. Trust boundary stripping (зависит от DataBag + Warning + Error)
6. Обновить `ErrorExtensions` / добавить `WarningExtensions`
7. Проверить и обновить `ResultExtensions` на совместимость с новой моделью
8. Обновить `StandardErrors` factories под `DataBag`
9. Обновить все тесты
10. Обновить `docs/HowToUse.md`
