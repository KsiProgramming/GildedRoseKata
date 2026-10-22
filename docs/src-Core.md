<!-- module: src/Core -->
# GildedRoseKata.Core (`src/Core`)

This is the core library of the Gilded Rose kata solution. It holds two implementations of the kata's "update quality at end of day" rule. One is the original legacy code. The other is a refactored, domain-modelled version, which the code calls "ProposedCode". The kata rules are in the repository `README.md`.

## Where it fits

- The project is `GildedRoseKata.Core.csproj`, a class library with the root namespace `GildedRoseKata`.
- `Directory.Build.props` at the repository root supplies the shared settings: `net8.0`, `LangVersion latest`, implicit usings, nullable reference types, and the Roslynator and SonarAnalyzer analyzers. Analyzers are disabled in the Release configuration, and `TreatWarningsAsErrors` is `false`.
- The test project `tests/Core.Tests` (`GildedRoseKata.Core.Tests`) references this project. It uses xUnit and FluentAssertions, with coverlet for coverage. It contains `ProposedCode/ItemMaturationServiceTests.cs` and a `ProposedCode/Domain/` folder.
- The module's `.csproj` only overrides the analyzer package versions (`Roslynator.Analyzers` 5.0.0, `SonarAnalyzer.CSharp` 10.33.0.1635) and clears the `CodeAnalysisRuleSet` property. It has no other package dependencies.

## Structure

```
src/Core/
├── GildedRoseKata.Core.csproj
├── OriginalCode/                 (namespace GildedRoseKata.OriginalCode)
│   ├── Item.cs
│   └── UpdateQualityService.cs
└── ProposedCode/                 (namespace GildedRoseKata.ProposedCode)
    ├── IMaturingPolicy.cs
    ├── ItemMaturationService.cs
    └── Domain/
        ├── ItemKind.cs
        ├── ItemMaturingPolicy.cs
        ├── ItemState.cs
        ├── Quality.cs
        └── SellIn.cs
```

The files under `ProposedCode/Domain/` still declare the `GildedRoseKata.ProposedCode` namespace.

## OriginalCode

- **`Item`** is a mutable class with `Name`, `SellIn` and `Quality`. The kata says this class must not be altered. The refactored code reuses it as its input and output type.
- **`UpdateQualityService(IList<Item> items)`** takes the item list in its primary constructor. `UpdateQuality()` mutates the items in place with the original nested-`if` logic. It matches item names as literal strings: "Aged Brie", "Backstage passes to a TAFKAL80ETC concert" and "Sulfuras, Hand of Ragnaros". Warnings are suppressed around the method with `#pragma warning disable`. This version has no Conjured handling.

## ProposedCode

The refactored version separates classifying an item, modelling its state, and the maturing rules.

### Public API

- **`ItemMaturationService`** is the entry point.
  - Its constructors are `ItemMaturationService()`, which uses `ItemMaturingPolicy` by default, and `ItemMaturationService(IMaturingPolicy)`.
  - `Item Mature(Item item)` does not mutate its input. It classifies the item with `ItemKind.FromName`, builds an `ItemState`, applies the policy, and returns a new `Item` with the same `Name` and the updated `Quality` and `SellIn`.
- **`IMaturingPolicy`** is the interface `ItemState Mature(ItemKind kind, ItemState state)`.
- **`ItemMaturingPolicy`** is the default implementation of `IMaturingPolicy`. It switches on the item kind:

| Kind | Rule |
|---|---|
| Standard | Quality −1 per day, and −1 more once the sell-in has expired. |
| AgedBrie | Quality +1 per day, and +1 more once expired. |
| Backstage | Quality +1 per day. It gains +2 when the current sell-in is below 11 and +3 when it is below 6. Quality resets to 0 once the sell-in has expired. |
| Conjured | Quality −2 per day, and −2 more once expired. |
| Sulfuras | The state is returned unchanged. |

- **`ItemKind`** is an abstract record with nested sealed records `Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`. `ItemKind.FromName(string)` maps names to kinds:
  - "Aged Brie" and "AgedBrie" map to `AgedBrie`.
  - "Backstage passes to a TAFKAL80ETC concert" and "Backstage" map to `Backstage`.
  - "Conjured Mana Cake" and "Conjured" map to `Conjured`.
  - "Sulfuras, Hand of Ragnaros" maps to `Sulfuras`.
  - Any other name maps to `Standard`.
- **`ItemState`** is a readonly record struct holding a `Quality` and a `SellIn`. `ItemState.Create(kind, quality, sellIn)` builds one from raw ints.
- **`Quality`** is a readonly record struct that wraps an int.
  - The constants are `Minimum = 0`, `Maximum = 50` and `Legendary = 80`.
  - The public constructor clamps the value to 0–50.
  - `FromItemKind` gives Sulfuras the fixed value 80 through a private constructor that bypasses the clamp.
  - `Increase(amount = 1)` and `Decrease(amount = 1)` clamp the result. They throw `ArgumentOutOfRangeException` for a negative amount.
  - `Reset()` returns 0.
- **`SellIn`** is a readonly record struct that wraps an int.
  - `IsExpired` is true when the value is below 0.
  - `IsBelow(limit)` tests whether the value is below the limit.
  - `NextDay()` decrements the value.

## Usage

```csharp
using GildedRoseKata.OriginalCode;
using GildedRoseKata.ProposedCode;

var service = new ItemMaturationService();
Item next = service.Mature(new Item { Name = "Aged Brie", SellIn = 2, Quality = 0 });
```

To use the legacy behaviour, call `new UpdateQualityService(items).UpdateQuality()` on an `IList<Item>`.

## Build and test

The repository root has `GildedRoseKata.sln`. The standard .NET CLI commands apply:

```
dotnet build src/Core
dotnet test tests/Core.Tests
```

CI is defined in `.github/workflows/dotnet.yml`, which calls the reusable `build.yml` workflow with .NET `9.0.x` on pushes and pull requests to `master`.

## Notes

- The two implementations differ in behaviour. For example, the original has no Conjured rule, while the proposed one does.
- `ItemMaturingPolicy` is injected through `IMaturingPolicy`, so the rules can be swapped or tested without changing `ItemMaturationService`.
