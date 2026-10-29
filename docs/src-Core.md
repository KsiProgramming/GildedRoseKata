<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

## Purpose

`GildedRoseKata.Core` is the class library that holds the business logic of the Gilded Rose kata. It contains two implementations of the "update item quality at end of day" rule:

- **`OriginalCode`** is the legacy implementation, a nested-`if` loop over a list of items.
- **`ProposedCode`** is a refactored, domain-modelled replacement. It uses value types, a kind hierarchy and a policy object.

The project is listed in `GildedRoseKata.sln` as `GildedRoseKata.Core`. The test project `tests/Core.Tests` references it. The target framework (`net8.0`) comes from the repository-level `Directory.Build.props`.

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

The folder is `Domain/`, but all `ProposedCode` types are in the namespace `GildedRoseKata.ProposedCode`. The `OriginalCode` types are in `GildedRoseKata.OriginalCode`.

## OriginalCode (legacy)

- **`Item`** is a mutable class with `Name`, `SellIn` and `Quality`. The nullable warning on `Name` is suppressed.
- **`UpdateQualityService(IList<Item> items)`** has a single method, `UpdateQuality()`. It mutates every item in place with deeply nested conditionals that match on hard-coded item names. Warnings are suppressed around the method.

## ProposedCode (refactored)

### Public entry point

**`ItemMaturationService`** has two constructors:
- The default constructor uses `ItemMaturingPolicy`.
- The second takes an `IMaturingPolicy`.

`Item Mature(Item item)` does the following:
1. It maps `item.Name` to an `ItemKind`.
2. It builds an `ItemState` from the item's quality and sell-in.
3. It asks the policy to mature that state.
4. It returns a **new** `Item` with the same name and the updated values. The input item is not modified.

It reuses the `Item` class from `OriginalCode`.

### Policy

- **`IMaturingPolicy`** defines `ItemState Mature(ItemKind kind, ItemState state)`.
- **`ItemMaturingPolicy`** is the default implementation. It switches on the kind:

| Kind | Quality rule per day |
|---|---|
| Standard | Decreases by 1. Decreases by 1 more once sell-in is expired. |
| AgedBrie | Increases by 1. Increases by 1 more once expired. |
| Backstage | Uses the current sell-in: +3 if below 6, +2 if below 11, otherwise +1. Resets to 0 once the new sell-in is expired. |
| Conjured | Decreases by 2. Decreases by 2 more once expired. |
| Sulfuras | State returned unchanged. |

### Domain types

- **`ItemKind`** is an abstract record with nested sealed records `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`. `ItemKind.FromName(string)` maps these names:
  - `"Aged Brie"` and `"AgedBrie"` map to `AgedBrie`.
  - `"Backstage passes to a TAFKAL80ETC concert"` and `"Backstage"` map to `Backstage`.
  - `"Conjured Mana Cake"` and `"Conjured"` map to `Conjured`.
  - `"Sulfuras, Hand of Ragnaros"` maps to `Sulfuras`.
  - Any other name maps to `Standard`.
- **`ItemState`** is a readonly record struct holding a `Quality` and a `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one from raw ints.
- **`Quality`** is a readonly record struct whose value is clamped to `Minimum = 0` and `Maximum = 50`. `Legendary = 80` is the constant for Sulfuras. It offers:
  - `Increase(int amount = 1)` and `Decrease(int amount = 1)`, which throw `ArgumentOutOfRangeException` for negative amounts.
  - `Reset()`, which returns a quality of 0.
  - `FromItemKind(kind, value)`, which returns quality 80 for Sulfuras and a clamped value otherwise.
- **`SellIn`** is a readonly record struct with `Value`, `IsExpired` (value < 0), `IsBelow(int limit)` and `NextDay()`, which subtracts 1.

## Building and testing

The project is built as part of the solution:

```
dotnet build GildedRoseKata.sln
dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj
```

The tests in `tests/Core.Tests` include `ProposedCode/ItemMaturationServiceTests.cs`.

## Dependencies

The csproj has no runtime package dependencies. It only sets `PackageReference Update` entries for the analyzers `Roslynator.Analyzers` (5.0.0) and `SonarAnalyzer.CSharp` (10.33.0.1635), and it clears `CodeAnalysisRuleSet` for Debug and Release. The source files carry a "KsiProgramming" copyright header.
