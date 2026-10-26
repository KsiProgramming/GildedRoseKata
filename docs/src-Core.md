<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

This is the class library for the Gilded Rose kata. It holds two implementations of the "update quality" rules side by side:

- **`OriginalCode`** is the legacy implementation, kept as the starting point.
- **`ProposedCode`** is a refactored, domain-modelled replacement.

The project is `GildedRoseKata.Core.csproj` and is part of `GildedRoseKata.sln`. The test project `tests/Core.Tests/GildedRoseKata.Core.Tests.csproj` references it.

## Build configuration

- The `.csproj` sets very little. Shared settings come from the root `Directory.Build.props`.
  - Target framework is `net8.0`.
  - Root namespace is `GildedRoseKata`.
  - `LangVersion` is `latest`.
  - Implicit usings and nullable reference types are enabled.
  - `TreatWarningsAsErrors` is `false`.
  - Analyzers are switched off in Release builds.
- The analyzer packages are `Roslynator.Analyzers` and `SonarAnalyzer.CSharp`. They are referenced in `Directory.Build.props`, and the `.csproj` updates their versions through `PackageReference Update`.
- The `.csproj` does not define build or test commands. You can build it with the standard `dotnet build src/Core` command, or build the whole solution.

## OriginalCode (`GildedRoseKata.OriginalCode`)

- **`Item`** is a mutable class with `Name` (string), `SellIn` (int) and `Quality` (int).
- **`UpdateQualityService(IList<Item> items)`** has a single method, `UpdateQuality()`. It mutates the items in place with deeply nested conditionals. Warnings are suppressed with `#pragma warning disable`. The rules are keyed on item names:
  - `Aged Brie`
  - `Backstage passes to a TAFKAL80ETC concert`
  - `Sulfuras, Hand of Ragnaros`
  - everything else

## ProposedCode (`GildedRoseKata.ProposedCode`)

The refactor separates three concerns: classifying an item, holding its state, and applying the maturation rules.

### Entry point

**`ItemMaturationService`**
- Its constructors are a parameterless one, which uses `ItemMaturingPolicy`, and one that takes an `IMaturingPolicy`.
- `Item Mature(Item item)` maps the item name to an `ItemKind`. It builds an `ItemState` from the item's quality and sell-in and asks the policy to mature that state. It then returns a **new** `Item` with the same name and the updated values. The input item is not mutated.
- `Item` comes from `OriginalCode`, so the proposed code depends on the original model type.

### Policy

- **`IMaturingPolicy`** declares `ItemState Mature(ItemKind kind, ItemState state)`.
- **`ItemMaturingPolicy`** is the default implementation. It switches on the kind:

| Kind | Daily rule |
|---|---|
| Standard | Quality −1, and a further −1 once sell-in is expired. |
| AgedBrie | Quality +1, and a further +1 once expired. |
| Backstage | Quality +1 normally, +2 when sell-in is below 11, +3 when it is below 6 (checked against the sell-in before the day passes). Quality resets to 0 once expired. |
| Conjured | Quality −2, and a further −2 once expired. |
| Sulfuras | State is returned unchanged. |

### Domain types (`ProposedCode/Domain`)

- **`ItemKind`** is an abstract record with nested sealed records `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`.
  - `ItemKind.FromName(string)` maps names to kinds. It accepts the full kata names and the short forms `AgedBrie`, `Backstage` and `Conjured`. The full name `Conjured Mana Cake` is also accepted. Any other name becomes `Standard`.
- **`ItemState`** is a readonly record struct with `Quality` and `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one from raw integers.
- **`Quality`** is a readonly record struct.
  - Constants: `Minimum = 0`, `Maximum = 50`, `Legendary = 80`.
  - The constructor clamps values to 0–50.
  - `FromItemKind` gives Sulfuras the fixed legendary value of 80.
  - Methods: `Increase(int amount = 1)`, `Decrease(int amount = 1)` and `Reset()`. A negative amount throws `ArgumentOutOfRangeException`.
- **`SellIn`** is a readonly record struct. It has `IsExpired` (value < 0), `IsBelow(int limit)` and `NextDay()`.

## Dependencies

There are no runtime package dependencies. Only the analyzer packages mentioned above are used.
