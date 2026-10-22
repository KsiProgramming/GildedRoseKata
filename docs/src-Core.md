<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

`src/Core` is the only production library in the Gilded Rose Kata repository. It builds as the assembly `GildedRoseKata.Core`. It holds two things side by side:

- the legacy `UpdateQuality` implementation from the kata, unchanged, and
- a refactored "proposed" implementation. It adds the "Conjured" item rule and models the domain with immutable value types.

The repository's README describes the kata rules. The sections below follow a trimmed arc42 structure, covering only what the files show. The repository has no information on cloud hosting, regulation, disaster recovery or runtime monitoring, so those topics are left out.

## 1. Introduction and goals

| Goal | How the module addresses it |
|---|---|
| Keep the legacy behaviour available | `OriginalCode/` keeps the kata's nested-`if` `UpdateQuality` as the reference implementation. |
| Support the new "Conjured" category | Only `ProposedCode/` implements it. The legacy code has no Conjured branch. |
| Make the rules readable and testable | Each item category is a separate method. Quality and sell-in limits are enforced by value types. |
| Keep the kata's `Item` class | `ProposedCode` takes `OriginalCode.Item` as input and returns a new `Item` without modifying the original. |

The kata statement says the `Item` class "belongs to the goblin" and must not change. Both implementations therefore share the unchanged `Item` POCO.

## 2. Constraints

- **Target framework:** `net8.0`, set in the root `Directory.Build.props`.
- **Language and project settings:** the same file sets `LangVersion` to `latest`, enables implicit usings and nullable reference types, and sets `TreatWarningsAsErrors` to `false`.
- **Static analysis:** `Roslynator.Analyzers` and `SonarAnalyzer.CSharp` are referenced for every project through `Directory.Build.props`. `RunAnalyzers` is `false` in Release. The module's `.csproj` updates their versions to Roslynator 5.0.0 and SonarAnalyzer 10.33.0.1635.
- **Kata rules:** quality is between 0 and 50, except that Sulfuras is fixed at 80. The README lists the other rules.
- **CI SDK:** the GitHub workflow installs .NET SDK `9.0.x` (`dotnet.yml`), while the projects target `net8.0`.

## 3. Context and scope

```text
   Caller (host app / tests)
        |
        |  Item (Name, SellIn, Quality)
        v
+--------------------------- GildedRoseKata.Core ---------------------------+
|  OriginalCode.UpdateQualityService   (legacy, mutates IList<Item>)        |
|  ProposedCode.ItemMaturationService  (new, returns a new Item)            |
+---------------------------------------------------------------------------+
```

- The module has no external system integrations. It has no I/O, no configuration and no package dependencies of its own beyond the analyzers.
- The only consumer in the repository is the test project `tests/Core.Tests`. It references this project and tests the `ProposedCode` types. No executable host is present in the repository.

## 4. Solution strategy

- **Parallel implementations.** `OriginalCode` stays as it was. `ProposedCode` is a rewrite in the same assembly, in a separate namespace.
- **Strategy pattern.** `IMaturingPolicy` abstracts the per-category rules. `ItemMaturationService` receives it by constructor injection and defaults to `ItemMaturingPolicy`.
- **Closed set of kinds.** `ItemKind` is an abstract record with sealed nested record subtypes. Policies pattern-match on it.
- **Immutable value objects.** `Quality`, `SellIn` and `ItemState` are `readonly record struct`s. Every operation returns a new value.
- **Invariants live in the types.** The `Quality` constructor clamps values to 0–50, so policies don't check bounds.

## 5. Building block view

```text
src/Core
├── GildedRoseKata.Core.csproj
├── OriginalCode/
│   ├── Item.cs                    Item POCO (Name, SellIn, Quality)
│   └── UpdateQualityService.cs    Legacy in-place update
└── ProposedCode/
    ├── IMaturingPolicy.cs         Strategy interface
    ├── ItemMaturationService.cs   Façade / entry point
    └── Domain/
        ├── ItemKind.cs            Category + name mapping
        ├── ItemMaturingPolicy.cs  Rules per category
        ├── ItemState.cs           (Quality, SellIn) pair
        ├── Quality.cs             Bounded quality value
        └── SellIn.cs              Days-to-sell value
```

Namespaces are `GildedRoseKata.OriginalCode` and `GildedRoseKata.ProposedCode`. The files under `ProposedCode/Domain/` declare the `GildedRoseKata.ProposedCode` namespace, not a `.Domain` sub-namespace.

### 5.1 OriginalCode

- **`Item`**: a class with settable `Name` (string), `SellIn` (int) and `Quality` (int).
- **`UpdateQualityService(IList<Item> items)`**: its primary constructor takes the list. `UpdateQuality()` loops over the list and **mutates each item in place**. Rules are hard-coded against the exact names `"Aged Brie"`, `"Backstage passes to a TAFKAL80ETC concert"` and `"Sulfuras, Hand of Ragnaros"`. The file disables compiler warnings with `#pragma warning disable`. There is no Conjured handling.

### 5.2 ProposedCode

| Type | Kind | Responsibility |
|---|---|---|
| `ItemMaturationService` | class | Public entry point. `Item Mature(Item item)` maps the item to a kind and state, calls the policy, and returns a **new** `Item` with the same `Name`. Constructors: parameterless (uses `ItemMaturingPolicy`) and `(IMaturingPolicy)`. |
| `IMaturingPolicy` | interface | `ItemState Mature(ItemKind kind, ItemState state)`. |
| `ItemMaturingPolicy` | class | Default `IMaturingPolicy`. It switches on the kind and applies the rules below. |
| `ItemKind` | abstract record | Subtypes `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`. `FromName(string)` maps names to kinds. |
| `ItemState` | readonly record struct | Holds `Quality` and `SellIn`. `Create(kind, quality, sellIn)` builds one. |
| `Quality` | readonly record struct | `Minimum` = 0, `Maximum` = 50, `Legendary` = 80. Methods `Increase(int = 1)`, `Decrease(int = 1)` and `Reset()`. |
| `SellIn` | readonly record struct | `Value`, `IsExpired` (`Value < 0`), `IsBelow(int)` and `NextDay()`. |

#### Name-to-kind mapping (`ItemKind.FromName`)

| Name | Kind |
|---|---|
| `Aged Brie`, `AgedBrie` | `AgedBrie` |
| `Backstage passes to a TAFKAL80ETC concert`, `Backstage` | `Backstage` |
| `Conjured Mana Cake`, `Conjured` | `Conjured` |
| `Sulfuras, Hand of Ragnaros` | `Sulfuras` |
| anything else | `Standard` |

Matching is a case-sensitive exact string match.

#### Rules in `ItemMaturingPolicy`

Here `expired` means the new sell-in is below 0.

| Kind | Quality change per day | Once expired | Sell-in |
|---|---|---|---|
| Standard | −1 | a further −1 | −1 |
| AgedBrie | +1 | a further +1 | −1 |
| Conjured | −2 | a further −2 | −1 |
| Backstage | +1. +2 if the old sell-in is below 11. +3 if it is below 6. | reset to 0 | −1 |
| Sulfuras | unchanged | unchanged | unchanged (the state is returned as-is) |

`Quality` clamps every result to 0–50.

#### Invariants and error handling

- `new Quality(value)` clamps to 0–50.
- `Quality.FromItemKind` returns 80 for Sulfuras whatever the input. For other kinds it clamps the input. `ItemMaturationService` therefore normalises an out-of-range input quality before maturing, for example 60 becomes 50.
- `Increase` and `Decrease` throw `ArgumentOutOfRangeException` for a negative amount.
- `SellIn` has no bounds.
- `ItemMaturationService.Mature` does not check its argument for `null`. A null `item` throws when `item.Name` is read.

## 6. Runtime scenario: maturing one item

```text
Caller -> ItemMaturationService.Mature(item)
  1. ItemKind.FromName(item.Name)                   -> kind
  2. ItemState.Create(kind, item.Quality, item.SellIn)
        Quality.FromItemKind(...) clamps, or sets 80 for Sulfuras
  3. IMaturingPolicy.Mature(kind, state)            -> matured state
        e.g. Backstage: quality +3 if SellIn < 6, then sell-in - 1,
             and quality = 0 if the new sell-in < 0
  4. return new Item { Name, Quality, SellIn }      (input item is not modified)
```

The legacy path is different. `new UpdateQualityService(list).UpdateQuality()` changes every `Item` in the list.

## 7. Building, running and testing

The repository shows no run target for this library, which has no entry point. Commands follow the CI workflow (`.github/workflows/build.yml`), run from the repository root:

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

The tests are in `tests/Core.Tests` (`GildedRoseKata.Core.Tests`). They use xUnit 2.9.3, FluentAssertions 8.10.0, Microsoft.NET.Test.Sdk and the coverlet.collector coverage collector. Test files cover `ItemKind`, `ItemMaturingPolicy`, `Quality`, `SellIn` and `ItemMaturationService`. The `OriginalCode` types have no test files in the repository.

## 8. Cross-cutting concerns

- **Logging, security, caching:** none in this module.
- **Concurrency:** the value types and `ItemMaturingPolicy` are stateless and immutable. `UpdateQualityService` mutates the list and items it is given.
- **Code style:** every source file has a copyright header ("KsiProgramming"). The root `.editorconfig` and the two analyzer packages govern style and analysis.

## 9. Design decisions evident in the code

| Decision | Evidence | Consequence |
|---|---|---|
| Keep the legacy code beside the rewrite | Both `OriginalCode/` and `ProposedCode/` exist in the same project | The old behaviour stays available for comparison. The assembly contains two implementations. |
| Strategy interface for rules | `IMaturingPolicy` and a second constructor on the service | The rules can be replaced or faked in tests. |
| Clamp in the value type | `Quality` constructor | Policies stay short. Bounds can't be bypassed except through the Sulfuras path. |
| Closed record hierarchy for kinds | `ItemKind` | A new category needs a new record, a name mapping and a policy branch. |
| Return a new `Item` | `ItemMaturationService.Mature` | No side effects on the input. The caller must replace the old item. |

## 10. Risks and technical debt

- **Duplicate behaviour.** Two implementations coexist, and the legacy one differs from the new one (no Conjured rule). Nothing in the repository tests that they agree.
- **Tests cover only `ProposedCode`.**
- **Name matching.** Item categories come from exact, case-sensitive names. Other Conjured names, such as `"Conjured Elixir"`, are treated as `Standard`.
- **Backstage thresholds.** The values 6 and 11 are literals in `ItemMaturingPolicy`.
- **Framework mismatch.** The projects target `net8.0`, but CI installs SDK 9.0.x.
- **Warnings.** `TreatWarningsAsErrors` is `false`, and `UpdateQualityService` suppresses warnings.
- **Typo in a property name.** `Directory.Build.props` defines `AcceleratBuilnInVisualStudio`, which looks like a misspelling. The repository gives no sign that anything uses it.
- **Minor.** The `.csproj` only sets `PackageReference Update` entries for the analyzers. The `Include` entries are in `Directory.Build.props`.
