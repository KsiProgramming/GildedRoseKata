<!-- module: tests/Core.Tests -->
# GildedRoseKata.Core.Tests

This is the xUnit test project for the Gilded Rose kata core library, `src/Core/GildedRoseKata.Core.csproj`. It tests the "proposed" (refactored) implementation, which lives in the `GildedRoseKata.ProposedCode` namespace. The project is not packable (`IsPackable=false`) and is flagged as a test project (`IsTestProject=true`).

## Layout

```
tests/Core.Tests/
├── GildedRoseKata.Core.Tests.csproj
└── ProposedCode/
    ├── ItemMaturationServiceTests.cs
    └── Domain/
        ├── ItemKindTests.cs
        ├── ItemMaturingPolicyTests.cs
        ├── QualityTests.cs
        └── SellInTests.cs
```

All test classes are in the namespace `GildedRoseKata.ProposedCode.Tests`. Each file starts with a copyright header for "KsiProgramming".

## What is tested

The tests are data-driven. They use `[Theory]` with `[InlineData]` for most cases and `[Fact]` for single cases. Assertions use FluentAssertions.

| Test class | Subject | Behaviour covered |
|---|---|---|
| `ItemMaturationServiceTests` | `ItemMaturationService.Mature(Item)` | End-to-end check on a legacy `GildedRoseKata.OriginalCode.Item`. It checks the resulting `Quality` and `SellIn` for each kind of item name. |
| `ItemKindTests` | `ItemKind.FromName` | Maps item names to `ItemKind.Standard`, `AgedBrie`, `Backstage`, `Conjured` and `Sulfuras`. Both full and short names resolve, for example `"Aged Brie"` and `"AgedBrie"`, or `"Backstage"` and the full TAFKAL80ETC passes name. |
| `ItemMaturingPolicyTests` | `ItemMaturingPolicy.Mature(kind, ItemState)` | Per-kind daily rules, using `ItemState(new Quality(..), new SellIn(..))`. The tests cover:<br>• Standard items.<br>• Aged Brie, which gains quality and is capped at 50.<br>• Backstage passes, which gain different amounts as the sell-in date nears and drop to 0 after the concert.<br>• Conjured items, which degrade by 2 per day.<br>• Sulfuras, whose state is preserved unchanged. |
| `QualityTests` | `Quality` value type | The constructor clamps values to 0–50. `Increase` and `Decrease` respect the bounds. `Reset` returns 0. `Quality.FromItemKind` returns `Quality.Legendary` for Sulfuras. |
| `SellInTests` | `SellIn` value type | `IsExpired` is true only for strictly negative values. `IsBelow(limit)` is true only when the value is below the limit. `NextDay()` decrements by one. |

### Rules the tests show

- Quality is clamped to 0–50. An item built through `ItemState.Create(new ItemKind.Sulfuras(), 80, 0)` keeps quality 80, and `Quality.FromItemKind` gives Sulfuras `Quality.Legendary`.
- In the service tests, `"Elixir of the Mongoose"` (quality 10, sell-in 5) becomes quality 9 and sell-in 4.
- `"Conjured"` with quality 10 and sell-in 0 becomes quality 6 and sell-in -1. Conjured items degrade twice as fast, and again twice as fast once expired.
- `"Sulfuras, Hand of Ragnaros"` (quality 80, sell-in 0) is unchanged.
- Backstage passes gain +1 above 10 days, +2 at 10 days or fewer, and +3 at 5 days or fewer. Quality drops to 0 once sell-in is 0 and the day has passed.

## Dependencies

Project reference:
- `src/Core/GildedRoseKata.Core.csproj`

NuGet packages:
- `xunit` 2.9.3
- `xunit.runner.visualstudio` 4.0.0
- `Microsoft.NET.Test.Sdk` 18.9.0
- `FluentAssertions` 8.10.0
- `coverlet.collector` 10.0.1, for code coverage
- `Roslynator.Analyzers` 5.0.0 and `SonarAnalyzer.CSharp` 10.33.0.1635. These are version overrides (`PackageReference Update`) for analyzers that are not otherwise referenced in this file.

`Xunit` is added as a global `using` in the project file.

## Running the tests

The repository does not show a dedicated script. Use the standard .NET CLI:

```sh
dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj
```

To collect coverage with the referenced collector, add `--collect:"XPlat Code Coverage"`.
