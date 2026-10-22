<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

`src/Core` is the class library (`GildedRoseKata.Core.csproj`) that holds all the production code of the Gilded Rose kata solution. It contains two implementations of the kata's "update quality" rule:

- **`OriginalCode`** is the legacy, deeply nested implementation. It is the code the kata asks you to refactor.
- **`ProposedCode`** is a refactored, domain-modelled replacement. It also supports "Conjured" items.

The kata rules are in the repository `README.md`. Examples are the 0–50 quality range, the sell-by date, Aged Brie, Sulfuras, Backstage passes and Conjured items.

## Place in the repository

- The project is part of `GildedRoseKata.sln`.
- `tests/Core.Tests` (`GildedRoseKata.Core.Tests.csproj`) is the test project. It references this project. It uses xUnit, FluentAssertions and coverlet. Its tests are under `ProposedCode/`, including `ItemMaturationServiceTests.cs`.
- Shared build settings come from the root `Directory.Build.props`:
  - target framework `net8.0`
  - root namespace `GildedRoseKata`
  - `LangVersion` latest
  - implicit usings and nullable reference types enabled
  - `TreatWarningsAsErrors` false
  - Roslynator and SonarAnalyzer analyzers added to every project; `RunAnalyzers` is off in Release
- `GildedRoseKata.Core.csproj` only adds analyzer package version updates (Roslynator.Analyzers 5.0.0, SonarAnalyzer.CSharp 10.33.0.1635). It has no other dependencies.

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

The `Domain` folder is only a directory. The types in it use the namespace `GildedRoseKata.ProposedCode`.

## OriginalCode (namespace `GildedRoseKata.OriginalCode`)

- **`Item`** is a mutable class with `Name` (string), `SellIn` (int) and `Quality` (int). The kata says this class must not be altered. The new code also uses it as its input and output type.
- **`UpdateQualityService(IList<Item> items)`** has a single method, `UpdateQuality()`. It mutates the items in place. Its logic is nested `if` statements that compare item names. The names it handles are `"Aged Brie"`, `"Backstage passes to a TAFKAL80ETC concert"` and `"Sulfuras, Hand of Ragnaros"`. It has no Conjured handling. Warnings are suppressed in this file with `#pragma warning disable`.

## ProposedCode (namespace `GildedRoseKata.ProposedCode`)

### Public API

- **`ItemMaturationService`** is the entry point.
  - `Mature(Item item)` returns a new `Item` with the same `Name` and updated `Quality` and `SellIn`. It does not modify the input.
  - Internally it maps the name to an `ItemKind`, builds an `ItemState`, and calls the policy.
  - The parameterless constructor uses `ItemMaturingPolicy`. A second constructor accepts any `IMaturingPolicy`.
- **`IMaturingPolicy`** has one method: `ItemState Mature(ItemKind kind, ItemState state)`.
- **`ItemMaturingPolicy`** is the default `IMaturingPolicy`. It switches on the kind and applies one day of aging:

| Kind | Daily rule in the code |
|---|---|
| Standard | Quality −1, and −1 more once the new SellIn is below 0 |
| AgedBrie | Quality +1, and +1 more once expired |
| Backstage | Quality +1 when SellIn ≥ 11, +2 when SellIn < 11, +3 when SellIn < 6 (checked against the SellIn before the day passes); quality resets to 0 once expired |
| Conjured | Quality −2, and −2 more once expired |
| Sulfuras | State returned unchanged |

### Domain types

- **`ItemKind`** is an abstract record with nested sealed records: `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`. `ItemKind.FromName(string)` maps item names to kinds. It accepts the full kata names and the short names `"AgedBrie"`, `"Backstage"` and `"Conjured"`. It also accepts `"Conjured Mana Cake"`. Any other name becomes `Standard`.
- **`ItemState`** is a readonly record struct of `Quality` and `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one from raw integers.
- **`Quality`** is a readonly record struct wrapping an int. It has constants `Minimum` = 0, `Maximum` = 50 and `Legendary` = 80, and the constructor clamps values to 0–50.
  - `FromItemKind` gives Sulfuras a fixed quality of 80.
  - `Increase(amount = 1)` and `Decrease(amount = 1)` return new values and throw `ArgumentOutOfRangeException` for negative amounts.
  - `Reset()` returns 0.
- **`SellIn`** is a readonly record struct wrapping an int. It has `IsExpired` (value < 0), `IsBelow(limit)` and `NextDay()`.

## Build and test

The repository shows no custom scripts, so the standard .NET CLI applies:

```
dotnet build GildedRoseKata.sln
dotnet test tests/Core.Tests
```

CI is defined in `.github/workflows/dotnet.yml`. It runs on pushes and pull requests to `master` and calls the reusable workflow `build.yml` with .NET `9.0.x`. The projects themselves target `net8.0`.
