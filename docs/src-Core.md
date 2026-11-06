<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

## Purpose

`src/Core` is the class library (`GildedRoseKata.Core.csproj`) for the Gilded Rose kata. It holds two implementations of the "update item quality" rules:

- **`OriginalCode`** is the legacy, deeply nested `UpdateQuality` implementation, kept as it was given in the kata.
- **`ProposedCode`** is a refactored, domain-modelled replacement. It uses value types, a discriminated-union-style item kind, and a pluggable policy.

The rules come from the kata description in the root `README.md`:

- Quality is never negative and never above 50.
- Aged Brie gains quality as it ages.
- Backstage passes gain 1, 2 or 3 quality per day, and drop to 0 after the concert.
- Sulfuras is legendary: quality 80, and it never changes.
- Conjured items degrade twice as fast.
- Quality degrades twice as fast once the sell-by date has passed.

All types use the `GildedRoseKata.*` namespaces (`GildedRoseKata.OriginalCode`, `GildedRoseKata.ProposedCode`), even for files under `ProposedCode/Domain`.

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

## Main parts

### OriginalCode

- **`Item`** is a mutable class with `Name`, `SellIn` and `Quality`. The kata says this class belongs to the "goblin" and must not be altered. The refactored code reuses it as its input and output type.
- **`UpdateQualityService(IList<Item> items)`** has one method, `UpdateQuality()`. It mutates the items in place and matches item names by string comparison. Warnings are suppressed around the method with `#pragma warning disable`.

### ProposedCode

- **`ItemMaturationService`** is the public entry point.
  - `ItemMaturationService()` uses the default `ItemMaturingPolicy`. A second constructor accepts any `IMaturingPolicy`.
  - `Item Mature(Item item)` does not mutate its input. It works in four steps:
    1. It maps the name to an `ItemKind`.
    2. It builds an `ItemState` from the item's quality and sell-in.
    3. It asks the policy to mature that state.
    4. It returns a new `Item` with the same `Name` and the updated `SellIn` and `Quality`.
- **`IMaturingPolicy`** has one method, `ItemState Mature(ItemKind kind, ItemState state)`. It is the extension point for the rules.
- **`ItemMaturingPolicy`** is the default implementation. It switches on the kind:

  | Kind | Behaviour per day |
  |---|---|
  | Standard | Quality −1, and a further −1 once the new sell-in is expired (< 0) |
  | AgedBrie | Quality +1, and a further +1 once expired |
  | Backstage | Uses the sell-in *before* the day passes: +3 if < 6, +2 if < 11, otherwise +1. Quality resets to 0 once the new sell-in is expired |
  | Conjured | Quality −2, and a further −2 once expired |
  | Sulfuras | State returned unchanged |

  Sell-in decreases by one per day for every kind except Sulfuras.

### Domain types (`ProposedCode/Domain`)

- **`ItemKind`** is an abstract record with sealed nested records `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`.
  - `ItemKind.FromName(string)` maps item names to kinds. Unknown names become `Standard`.
  - It accepts the full kata names, such as `"Aged Brie"`, `"Backstage passes to a TAFKAL80ETC concert"`, `"Conjured Mana Cake"` and `"Sulfuras, Hand of Ragnaros"`.
  - It also accepts the short aliases `"AgedBrie"`, `"Backstage"` and `"Conjured"`.
- **`Quality`** is a `readonly record struct`.
  - Constants: `Minimum = 0`, `Maximum = 50`, `Legendary = 80`.
  - The constructor clamps values to 0–50.
  - `Quality.FromItemKind(kind, value)` returns the fixed value 80 for Sulfuras and a clamped value otherwise.
  - `Increase(int amount = 1)`, `Decrease(int amount = 1)` and `Reset()` return new values. A negative amount throws `ArgumentOutOfRangeException`.
- **`SellIn`** is a `readonly record struct`.
  - It has `Value` and `IsExpired` (true when `Value < 0`).
  - `IsBelow(int limit)` and `NextDay()` are its other members.
- **`ItemState`** is a `readonly record struct` pairing a `Quality` and a `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one from raw integers.

## How it fits in the repository

- The solution file is `GildedRoseKata.sln` at the repository root.
- Tests are in `tests/Core.Tests` (`GildedRoseKata.Core.Tests.csproj`). They reference this project and cover `ItemKind`, `ItemMaturingPolicy`, `Quality`, `SellIn` and `ItemMaturationService` under `tests/Core.Tests/ProposedCode`. No tests for `OriginalCode` are present in that folder.
- Shared build settings come from the root `Directory.Build.props`:
  - target framework `net8.0`
  - `LangVersion` latest
  - implicit usings and nullable reference types enabled
  - `TreatWarningsAsErrors` false
  - analyzers disabled in Release

## Dependencies

The library has no runtime package dependencies. Two analyzer packages come from `Directory.Build.props`:

- `Roslynator.Analyzers` (4.12.10)
- `SonarAnalyzer.CSharp` (10.5.0.109200)

Both are private assets. The project file overrides their versions with `PackageReference Update` entries to 5.0.0 and 10.33.0.1635.

## Build and test

The repository contains no custom scripts. Standard .NET CLI commands apply:

```
dotnet build GildedRoseKata.sln
dotnet test tests/Core.Tests
```

CI is defined in `.github/workflows/dotnet.yml`. It runs on pushes and pull requests to `master` and calls the reusable `build.yml` workflow with .NET `9.0.x`.

## Usage example

```csharp
using GildedRoseKata.OriginalCode;
using GildedRoseKata.ProposedCode;

var service = new ItemMaturationService();
Item next = service.Mature(new Item { Name = "Aged Brie", SellIn = 2, Quality = 0 });
// next.SellIn == 1, next.Quality == 1
```
