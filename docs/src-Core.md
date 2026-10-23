<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

`src/Core` is the class library (`GildedRoseKata.Core.csproj`) that holds all the production code of the Gilded Rose kata solution. It contains two implementations of the "update the inventory at end of day" rule set:

- **`OriginalCode`** is the legacy implementation: a nested-`if` `UpdateQuality` method plus the `Item` class.
- **`ProposedCode`** is a refactored implementation. It models the rules with small domain types and a policy per item kind. It also implements the "Conjured" rule that the kata asks for.

The rules come from the kata description in the root `README.md`. Quality is never negative and never above 50. Quality degrades twice as fast after the sell-by date. Aged Brie gains quality. Backstage passes gain quality, then drop to 0 after the concert. Sulfuras is legendary, with quality 80. Conjured items degrade twice as fast.

## Layout

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

The namespaces are `GildedRoseKata.OriginalCode` and `GildedRoseKata.ProposedCode`. The files under `ProposedCode/Domain/` use the `GildedRoseKata.ProposedCode` namespace, not a `.Domain` namespace.

## OriginalCode

- **`Item`** is a mutable class with `Name`, `SellIn` and `Quality`. The kata says this class belongs to someone else and must not be altered. The refactored code still uses it as its input and output type.
- **`UpdateQualityService(IList<Item> items)`** has one public method, `UpdateQuality()`. It mutates the items in place. It is the original nested-conditional logic, with compiler warnings suppressed by `#pragma warning disable`. It has no Conjured handling. It compares names against literal strings for "Aged Brie", "Backstage passes to a TAFKAL80ETC concert" and "Sulfuras, Hand of Ragnaros".

## ProposedCode

### Public API

- **`ItemMaturationService`** is the entry point.
  - `Item Mature(Item item)` maps the item's name to an `ItemKind`. It builds an `ItemState`, asks the policy to mature it, and **returns a new `Item`** with the same name and the new values. The input item is not modified.
  - The parameterless constructor uses `ItemMaturingPolicy`. A second constructor takes any `IMaturingPolicy`.
- **`IMaturingPolicy`** declares `ItemState Mature(ItemKind kind, ItemState state)`.

### Domain types

- **`ItemKind`** is an abstract record with sealed nested records: `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`.
  - `ItemKind.FromName(string)` maps names to kinds. These names are recognised:
    - "Aged Brie" and "AgedBrie".
    - "Backstage passes to a TAFKAL80ETC concert" and "Backstage".
    - "Conjured Mana Cake" and "Conjured".
    - "Sulfuras, Hand of Ragnaros".
  - Any other name becomes `Standard`.
- **`ItemState`** is a readonly record struct pairing a `Quality` and a `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one from raw ints.
- **`Quality`** is a readonly record struct that wraps an int.
  - The constants are `Minimum = 0`, `Maximum = 50` and `Legendary = 80`.
  - The constructor clamps values to 0–50.
  - `FromItemKind` forces Sulfuras to 80 through a private constructor that bypasses the clamp.
  - The operations are `Increase(amount = 1)`, `Decrease(amount = 1)` and `Reset()`, which gives 0. Increase and Decrease throw `ArgumentOutOfRangeException` for a negative amount.
- **`SellIn`** is a readonly record struct that wraps an int. It provides `IsExpired` (value < 0), `IsBelow(limit)` and `NextDay()`.
- **`ItemMaturingPolicy`** is the default `IMaturingPolicy`. It switches on the kind:

| Kind | Daily change |
|---|---|
| Standard | Quality −1, and −1 more once the new SellIn is expired |
| AgedBrie | Quality +1, and +1 more once expired |
| Backstage | Quality +1 if SellIn ≥ 11, +2 if SellIn < 11, +3 if SellIn < 6. Quality is reset to 0 once the new SellIn is expired |
| Conjured | Quality −2, and −2 more once expired |
| Sulfuras | State unchanged |

SellIn decreases by one each day for every kind except Sulfuras.

Two details of the code are worth knowing:

- `Quality.Increase` and `Decrease` clamp to 0–50. A Sulfuras state is passed through unchanged, so it stays at 80.
- A non-Sulfuras item that is created with a quality above 50 is clamped to 50 when its state is built.

## Build and test

The project has no `Main`, so it is built as a library and consumed by tests.

- `Directory.Build.props` at the repository root sets `net8.0`, `LangVersion latest`, implicit usings and nullable reference types. Warnings are not treated as errors. It also adds the `Roslynator.Analyzers` and `SonarAnalyzer.CSharp` analyzers, which are switched off in the Release configuration. The `.csproj` pins newer analyzer versions through `PackageReference Update`.
- The solution file is `GildedRoseKata.sln`. Standard .NET CLI commands apply, for example `dotnet build` and `dotnet test`.
- The tests are in `tests/Core.Tests` (`GildedRoseKata.Core.Tests`). They use xUnit and FluentAssertions with coverlet, and they reference this project. The `ProposedCode` tests include `ItemMaturationServiceTests.cs` and a `Domain` folder.
- CI is defined in `.github/workflows/dotnet.yml`. It calls the reusable `build.yml` with .NET `9.0.x` on pushes and pull requests to `master`.

## Dependencies

The library has no runtime package dependencies. It uses only the .NET base class library, for example `System.Math`. The analyzer packages (Roslynator, SonarAnalyzer) are build-time only.
