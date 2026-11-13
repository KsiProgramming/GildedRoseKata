<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

## Purpose

`src/Core` is the class library (`GildedRoseKata.Core.csproj`) that holds all production code for the Gilded Rose kata. The kata is about updating the `SellIn` and `Quality` values of inn items at the end of each day. The rules are in the repository `README.md`.

The module holds two implementations of those rules side by side:

- **`OriginalCode`** is the legacy starting point of the kata. It is a nested-`if` `UpdateQuality` method that mutates items in place.
- **`ProposedCode`** is a refactored design. It splits the rules into small value types, an item-kind hierarchy and a replaceable policy. It does not mutate its input.

## Place in the repository

- The solution is `GildedRoseKata.sln`. This module is the only project under `src/`.
- The test project `tests/Core.Tests` (`GildedRoseKata.Core.Tests.csproj`) references this project. Its tests cover `ProposedCode`: `ItemKindTests`, `ItemMaturingPolicyTests`, `QualityTests`, `SellInTests` and `ItemMaturationServiceTests`. No tests for `OriginalCode` appear in the test folder.
- `ProposedCode` depends on `OriginalCode.Item` as its input and output type. `OriginalCode` does not depend on `ProposedCode`.

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

The `Domain` folder only groups files. Every type is in the namespace `GildedRoseKata.ProposedCode`, and the original types are in `GildedRoseKata.OriginalCode`.

## OriginalCode

- **`Item`** is a mutable class with `Name` (string), `SellIn` (int) and `Quality` (int). It is the shared data type, and the kata says it must not be altered.
- **`UpdateQualityService(IList<Item> items)`** takes the list in its primary constructor. `UpdateQuality()` walks the list and changes each item in place. It hard-codes the item names "Aged Brie", "Backstage passes to a TAFKAL80ETC concert" and "Sulfuras, Hand of Ragnaros". Warnings are suppressed around the method with `#pragma warning disable`. It has no handling for "Conjured" items.

## ProposedCode

### Public API

| Type | Role |
|---|---|
| `ItemMaturationService` | Entry point. `Item Mature(Item item)` returns a **new** `Item` with the same `Name` and the updated `Quality` and `SellIn`. It has a default constructor, which uses `ItemMaturingPolicy`, and a constructor that takes an `IMaturingPolicy`. |
| `IMaturingPolicy` | `ItemState Mature(ItemKind kind, ItemState state)`. This is the extension point for the rules. |
| `ItemMaturingPolicy` | The default `IMaturingPolicy`. It chooses a rule by `ItemKind`. |
| `ItemKind` | Abstract record with sealed nested records `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`. `ItemKind.FromName(string)` maps item names to kinds. |
| `ItemState` | Readonly record struct of `Quality` and `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one. |
| `Quality` | Readonly record struct for a quality value. |
| `SellIn` | Readonly record struct for days left to sell. |

### Flow

`ItemMaturationService.Mature` runs these steps:

1. It calls `ItemKind.FromName(item.Name)`.
2. It calls `ItemState.Create(kind, item.Quality, item.SellIn)`.
3. It calls `IMaturingPolicy.Mature(kind, state)`.
4. It copies the result into a new `Item`.

### Name mapping (`ItemKind.FromName`)

- `"Aged Brie"` and `"AgedBrie"` map to `AgedBrie`.
- `"Backstage passes to a TAFKAL80ETC concert"` and `"Backstage"` map to `Backstage`.
- `"Conjured Mana Cake"` and `"Conjured"` map to `Conjured`.
- `"Sulfuras, Hand of Ragnaros"` maps to `Sulfuras`.
- Any other name maps to `Standard`. The match is exact and case-sensitive.

### Value types

- **`Quality`**
  - Constants are `Minimum = 0`, `Maximum = 50` and `Legendary = 80`.
  - The public constructor clamps the value to 0–50.
  - `FromItemKind(kind, value)` returns the legendary value 80 for `Sulfuras` and a clamped value for any other kind.
  - `Increase(amount = 1)` and `Decrease(amount = 1)` return new, clamped values. A negative `amount` throws `ArgumentOutOfRangeException`.
  - `Reset()` returns 0.
- **`SellIn`**
  - `Value` holds the day count.
  - `IsExpired` is true when the value is below 0.
  - `IsBelow(limit)` tests whether the value is below `limit`.
  - `NextDay()` returns a copy with the value decreased by 1.

### Rules in `ItemMaturingPolicy`

Each rule moves `SellIn` forward one day. "Expired" means the new `SellIn` is below 0.

| Kind | Quality change |
|---|---|
| Standard | −1, and −1 more once expired. |
| AgedBrie | +1, and +1 more once expired. |
| Backstage | Uses the `SellIn` before the day passes: +3 if below 6, +2 if below 11, otherwise +1. Resets to 0 once expired. |
| Conjured | −2, and −2 more once expired. |
| Sulfuras | The state is returned unchanged, so `SellIn` does not change. |

Quality is always kept between 0 and 50 by `Quality`. The exception is Sulfuras, which is created with quality 80.

## Build and test

The repository does not document local commands beyond the CI workflow. `.github/workflows/build.yml` runs these commands from the repository root:

```
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

`dotnet.yml` triggers the workflow on pushes and pull requests to `master`, using .NET `9.0.x`.

## Dependencies and build configuration

- This project has no runtime package dependencies and no project references.
- `Directory.Build.props` at the repository root supplies the shared settings:
  - Target framework `net8.0`, `LangVersion` `latest`, implicit usings enabled and nullable reference types enabled.
  - `TreatWarningsAsErrors` is `false`.
  - Analyzers are disabled in the Release configuration.
  - The analyzer packages `Roslynator.Analyzers` and `SonarAnalyzer.CSharp` are referenced for every project.
- `GildedRoseKata.Core.csproj` only updates the versions of those two analyzer packages (Roslynator 5.0.0, Sonar 10.33.0.1635) and clears `CodeAnalysisRuleSet`.
- The test project uses xUnit, FluentAssertions and coverlet.
