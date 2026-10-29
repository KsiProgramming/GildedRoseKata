<!-- module: tests/Core.Tests -->
# GildedRoseKata.Core.Tests

This is the xUnit test project for `src/Core` (`GildedRoseKata.Core`), the Gilded Rose kata implementation. It tests the "proposed code" under `src/Core/ProposedCode`. That code is a domain model for aging items. The tests also feed it the `Item` class from `GildedRoseKata.OriginalCode`.

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

All test classes use the namespace `GildedRoseKata.ProposedCode.Tests`. The folders mirror the source layout under `src/Core/ProposedCode`.

## What is tested

| Test class | Subject | Behaviour covered |
|---|---|---|
| `ItemMaturationServiceTests` | `ItemMaturationService.Mature(Item)` | An end-to-end theory over item names. It checks the resulting `Quality` and `SellIn` for standard, Aged Brie, Backstage, Conjured and Sulfuras items. Short names such as `"Backstage"`, `"Conjured"` and `"AgedBrie"` resolve to the same kinds as the full names. |
| `ItemKindTests` | `ItemKind.FromName` | Maps names to the `ItemKind.AgedBrie`, `Backstage`, `Conjured`, `Sulfuras` and `Standard` types. The default is `Standard`. |
| `ItemMaturingPolicyTests` | `ItemMaturingPolicy.Mature(kind, ItemState)` | Per-kind daily rules, including boundary cases: |
| | | • Standard items lose 1 quality, or 2 once expired. |
| | | • Aged Brie gains quality, capped at 50. |
| | | • Backstage passes gain 1, 2 or 3 depending on days left, and drop to 0 after the concert. |
| | | • Conjured items degrade at double speed. |
| | | • Sulfuras state is returned unchanged. |
| `QualityTests` | `Quality` | The constructor clamps values to the range 0–50. `Increase` and `Decrease` respect those bounds. `Reset` returns 0. `FromItemKind` returns `Quality.Legendary` for Sulfuras. |
| `SellInTests` | `SellIn` | `IsExpired` is true only for strictly negative values. `IsBelow(limit)` and `NextDay()` (decrement by one) are also covered. |

The tests also use `ItemState` (constructed from `Quality` and `SellIn`, or through `ItemState.Create(kind, quality, sellIn)`).

## Building and running

The repository shows no custom scripts, so the standard .NET CLI applies:

```
dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj
```

`coverlet.collector` is referenced, so coverage can be collected with `--collect:"XPlat Code Coverage"`.

## Dependencies

Defined in `GildedRoseKata.Core.Tests.csproj`:

- Project reference: `..\..\src\Core\GildedRoseKata.Core.csproj`
- `xunit` 2.9.3, with `xunit.runner.visualstudio` 4.0.0 as the runner. `Xunit` is a global `using`.
- `Microsoft.NET.Test.Sdk` 18.9.0
- `FluentAssertions` 8.10.0, used for assertions throughout
- `coverlet.collector` 10.0.1 for code coverage
- Analyzer version overrides for `Roslynator.Analyzers` (5.0.0) and `SonarAnalyzer.CSharp` (10.33.0.1635). `CodeAnalysisRuleSet` is cleared in both Debug and Release.

The project is marked `IsTestProject=true` and `IsPackable=false`.

## Conventions

- Tests are mostly data-driven `[Theory]` / `[InlineData]` cases, named `Method_Condition_ExpectedBehaviour`.
- Each file starts with a copyright header (KsiProgramming).
