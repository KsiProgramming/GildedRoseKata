<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

## Purpose

`src/Core` is the only source project in this repository (`GildedRoseKata.Core.csproj`). It holds two implementations of the Gilded Rose kata's inventory-aging rules:

- **`OriginalCode`** is the legacy `UpdateQuality` implementation and the `Item` class from the kata.
- **`ProposedCode`** is a refactored, domain-modelled replacement for the legacy logic.

The rules come from the kata description in the root `README.md`:

- Quality is never negative and never above 50.
- Quality degrades twice as fast once the sell-by date has passed.
- Aged Brie gains quality as it ages.
- Backstage passes gain quality as the concert approaches, then drop to 0 after it.
- Sulfuras never changes and has a quality of 80.
- Conjured items degrade twice as fast as standard items.

## Place in the repository

```
GildedRoseKata.sln
Directory.Build.props           shared build settings (net8.0, nullable, analyzers)
src/Core/                       this module (GildedRoseKata.Core)
tests/Core.Tests/               xUnit test project, references src/Core
.github/workflows/              CI: dotnet.yml -> build.yml (restore, build, test)
```

The module has no project references of its own. `tests/Core.Tests` references it. The tests are under `tests/Core.Tests/ProposedCode/` and cover `ItemKind`, `ItemMaturingPolicy`, `Quality`, `SellIn` and `ItemMaturationService`.

## Structure

```
src/Core/
├── GildedRoseKata.Core.csproj
├── OriginalCode/
│   ├── Item.cs
│   └── UpdateQualityService.cs
└── ProposedCode/
    ├── IMaturingPolicy.cs
    ├── ItemMaturationService.cs
    └── Domain/
        ├── ItemKind.cs
        ├── ItemMaturingPolicy.cs
        ├── ItemState.cs
        ├── Quality.cs
        └── SellIn.cs
```

The `Domain/` folder is only a directory. The types in it use the namespace `GildedRoseKata.ProposedCode`, not a `.Domain` sub-namespace. `OriginalCode` types use `GildedRoseKata.OriginalCode`.

## Components

### Original code (`GildedRoseKata.OriginalCode`)

| Type | Role |
|---|---|
| `Item` | Mutable data holder with `string Name`, `int SellIn` and `int Quality`. The proposed code also uses it as its input and output type. |
| `UpdateQualityService(IList<Item> items)` | Primary-constructor class. `UpdateQuality()` loops over the items and mutates them in place with nested conditionals. The service is wrapped in `#pragma warning disable`/`restore`. |

### Proposed code (`GildedRoseKata.ProposedCode`)

| Type | Kind | Responsibility |
|---|---|---|
| `ItemMaturationService` | class | Public entry point. `Item Mature(Item item)` maps the item to a domain kind and state, applies the policy, and returns a **new** `Item`. The input is not mutated. There are two constructors: a parameterless one that uses `ItemMaturingPolicy`, and one that accepts any `IMaturingPolicy`. |
| `IMaturingPolicy` | interface | `ItemState Mature(ItemKind kind, ItemState state)`. This is the extension point for the aging rules. |
| `ItemMaturingPolicy` | class | Default `IMaturingPolicy`. It chooses a rule with a `switch` on the `ItemKind` subtype. |
| `ItemKind` | abstract record | Closed hierarchy of nested sealed records: `Standard`, `AgedBrie`, `Backstage`, `Conjured`, `Sulfuras`. `ItemKind.FromName(string)` maps names to kinds. |
| `ItemState` | readonly record struct | Pairs a `Quality` with a `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one. |
| `Quality` | readonly record struct | Value object for quality. See below. |
| `SellIn` | readonly record struct | Value object for days remaining. `IsExpired` is true when the value is below 0. It also has `IsBelow(int limit)` and `NextDay()`, which decrements the value. |

### Name mapping (`ItemKind.FromName`)

| Name | Kind |
|---|---|
| `"Aged Brie"`, `"AgedBrie"` | `AgedBrie` |
| `"Backstage passes to a TAFKAL80ETC concert"`, `"Backstage"` | `Backstage` |
| `"Conjured Mana Cake"`, `"Conjured"` | `Conjured` |
| `"Sulfuras, Hand of Ragnaros"` | `Sulfuras` |
| anything else | `Standard` |

Matching is exact and case-sensitive.

### `Quality` rules

- The constants are `Minimum = 0`, `Maximum = 50` and `Legendary = 80`.
- The public constructor clamps the value to 0–50.
- `Quality.FromItemKind(kind, value)` returns 80 for `Sulfuras`, using a private constructor that bypasses the clamp. For other kinds it clamps.
- `Increase(int amount = 1)` and `Decrease(int amount = 1)` throw `ArgumentOutOfRangeException` for a negative amount. Both clamp the result.
- `Reset()` returns 0.

### Maturing rules in `ItemMaturingPolicy`

Each rule decrements `SellIn` first, then adjusts quality again if the new `SellIn` is below 0. Backstage is the exception: its quality step depends on the old `SellIn`.

| Kind | Daily change | After expiry (new `SellIn` < 0) |
|---|---|---|
| Standard | Quality −1 | a further −1 |
| AgedBrie | Quality +1 | a further +1 |
| Backstage | Based on the old `SellIn`: +3 if below 6, +2 if below 11, otherwise +1 | Quality reset to 0 |
| Conjured | Quality −2 | a further −2 |
| Sulfuras | State returned unchanged | unchanged |

### Data flow

```
Item ──► ItemKind.FromName ──► ItemKind
  │
  └────► ItemState.Create(kind, quality, sellIn) ──► ItemState
                                                      │
              IMaturingPolicy.Mature(kind, state) ◄───┘
                          │
                          ▼
                  new ItemState ──► new Item { Name, Quality, SellIn }
```

## Build and test

The repository's CI (`.github/workflows/build.yml`) runs these commands from the repository root:

```
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

`dotnet.yml` triggers the build on pushes and pull requests to `master`. It passes `dotnet-version: 9.0.x`, while `Directory.Build.props` sets the target framework to `net8.0`.

## Configuration and dependencies

- **Target framework:** `net8.0`, from `Directory.Build.props`. The same file sets `LangVersion` to `latest`, `ImplicitUsings` and `Nullable` to enabled, and `TreatWarningsAsErrors` to `false`.
- **Analyzers:** `Roslynator.Analyzers` and `SonarAnalyzer.CSharp` are referenced for all projects through `Directory.Build.props`. They are disabled in Release configuration (`RunAnalyzers=false`). `GildedRoseKata.Core.csproj` uses `PackageReference Update` to override their versions to 5.0.0 and 10.33.0.1635.
- **Runtime dependencies:** none. The module uses only the base class library.
- **Test stack** (in `tests/Core.Tests`): xUnit 2.9.3, FluentAssertions 8.10.0, Microsoft.NET.Test.Sdk, coverlet.collector.

## Notes

- The `Item` class is shared across both implementations. The kata README says not to alter it, and the module leaves it as a plain mutable class.
- The tests in the repository target only `ProposedCode`. No tests for `UpdateQualityService` appear in `tests/Core.Tests`.
