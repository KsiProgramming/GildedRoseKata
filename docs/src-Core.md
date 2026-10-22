<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

This is the core class library of the Gilded Rose kata solution. It holds two implementations of the same inventory-update rules:

- **`OriginalCode`** is the legacy `UpdateQuality` implementation and the `Item` class. It is kept as it was handed over.
- **`ProposedCode`** is a refactored, domain-modelled version. It supports the "Conjured" item category that the kata asks for.

The root namespace is `GildedRoseKata`. The namespaces are `GildedRoseKata.OriginalCode` and `GildedRoseKata.ProposedCode`.

## Place in the repository

- The project file is `src/Core/GildedRoseKata.Core.csproj`, part of `GildedRoseKata.sln`.
- Shared build settings come from the root `Directory.Build.props`:
  - target framework `net8.0`
  - latest C# language version
  - implicit usings and nullable reference types enabled
  - Roslynator and SonarAnalyzer analyzers, which are switched off in Release
- The csproj also pins analyzer package versions with `PackageReference Update` (Roslynator.Analyzers 5.0.0, SonarAnalyzer.CSharp 10.33.0.1635).
- The test project `tests/Core.Tests` references this library. It uses xUnit and FluentAssertions, and includes `ProposedCode/ItemMaturationServiceTests.cs`.
- The CI workflow `.github/workflows/dotnet.yml` calls the reusable `build.yml` with .NET `9.0.x`.

The library has no runtime package dependencies of its own. Only the analyzers are referenced.

## Contents

### OriginalCode

| Type | Description |
|---|---|
| `Item` | Mutable class with `Name`, `SellIn` and `Quality`. The kata says this class must not be altered. |
| `UpdateQualityService(IList<Item> items)` | `UpdateQuality()` loops over the items and mutates them in place with deeply nested conditionals. Warnings are suppressed with `#pragma warning disable`. It has no handling for Conjured items. |

### ProposedCode

| Type | Description |
|---|---|
| `ItemMaturationService` | Public entry point. `Mature(Item)` returns a **new** `Item` with the same `Name` and the updated `SellIn` and `Quality`. The input item is not modified. The default constructor uses `ItemMaturingPolicy`. A second constructor accepts any `IMaturingPolicy`. |
| `IMaturingPolicy` | `ItemState Mature(ItemKind kind, ItemState state)`. |
| `ItemMaturingPolicy` | Default policy. It selects the rule by `ItemKind` with a `switch`. |
| `ItemKind` | Abstract record with sealed nested records `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`. `ItemKind.FromName(string)` maps item names to kinds. |
| `ItemState` | Readonly record struct holding a `Quality` and a `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one from raw ints. |
| `Quality` | Readonly record struct. The value is clamped to 0–50 (`Minimum`, `Maximum`). `Legendary` is 80. Sulfuras gets 80 through `Quality.FromItemKind`. `Increase(n)` and `Decrease(n)` throw `ArgumentOutOfRangeException` for negative amounts. `Reset()` returns 0. |
| `SellIn` | Readonly record struct with `Value`, `IsExpired` (`Value < 0`), `IsBelow(limit)` and `NextDay()`. |

The domain types are in `ProposedCode/Domain/`, but they use the `GildedRoseKata.ProposedCode` namespace.

#### Name-to-kind mapping

`ItemKind.FromName` recognises these names:

- `"Aged Brie"` and `"AgedBrie"` map to `AgedBrie`.
- `"Backstage passes to a TAFKAL80ETC concert"` and `"Backstage"` map to `Backstage`.
- `"Conjured Mana Cake"` and `"Conjured"` map to `Conjured`.
- `"Sulfuras, Hand of Ragnaros"` maps to `Sulfuras`.
- Any other name maps to `Standard`.

#### Maturing rules (`ItemMaturingPolicy`)

The rules run once per day. "Expired" means the new SellIn is below 0.

- **Standard**: Quality −1, with an extra −1 once expired.
- **Aged Brie**: Quality +1, with an extra +1 once expired.
- **Backstage**:
  - Quality rises by 1, or by 2 if the current SellIn is below 11, or by 3 if it is below 6.
  - Quality resets to 0 once expired.
- **Conjured**: Quality −2, with an extra −2 once expired.
- **Sulfuras**: the state is returned unchanged.
- SellIn goes down by 1 each day for every kind except Sulfuras.
- Quality is always kept within 0–50 by the `Quality` type.

## Usage

```csharp
using GildedRoseKata.OriginalCode;
using GildedRoseKata.ProposedCode;

var service = new ItemMaturationService();
Item next = service.Mature(new Item { Name = "Aged Brie", SellIn = 2, Quality = 0 });
```

To change the rules, pass a custom `IMaturingPolicy` to `new ItemMaturationService(policy)`.

## Build and test

These commands use the standard .NET SDK and the solution at the repository root:

```
dotnet build GildedRoseKata.sln
dotnet test GildedRoseKata.sln
```

## Differences between the two implementations

- The original code only compares the exact item names.
- `ProposedCode` also accepts the short names `"AgedBrie"`, `"Backstage"` and `"Conjured"`.
- `ProposedCode` supports Conjured items, which the original code does not.
- `ProposedCode` creates new `Item` instances instead of mutating them.
- `ProposedCode` forces a Sulfuras item's quality to 80 when it builds the state. Sulfuras is never modified after that.
