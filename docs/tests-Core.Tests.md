<!-- module: tests/Core.Tests -->
# GildedRoseKata.Core.Tests

The `tests/Core.Tests` project holds the xUnit test suite for `src/Core` (`GildedRoseKata.Core`). That library contains two implementations of the Gilded Rose Kata inventory update logic. These are the legacy `OriginalCode` and the refactored `ProposedCode`. The tests here cover the `ProposedCode` side: the domain value types, the item-kind resolution, the maturing policy and the service that ties them together.

## Role in the repository

- It is the only test project in `GildedRoseKata.sln`. It sits under `tests/` and mirrors the `src/Core` layout.
- It references `..\..\src\Core\GildedRoseKata.Core.csproj`.
- Its test files are in the `GildedRoseKata.ProposedCode.Tests` namespace. They exercise the `ProposedCode` types directly.
- `ItemMaturationServiceTests` also imports `GildedRoseKata.OriginalCode` to build the `Item` objects (`Name`, `Quality`, `SellIn`) that the service accepts.

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

## What is tested

All tests are xUnit `[Theory]` (with `[InlineData]`) or `[Fact]` cases. Assertions use FluentAssertions.

| Test class | Subject | Behaviour covered |
|---|---|---|
| `ItemMaturationServiceTests` | `ItemMaturationService.Mature(Item)` | End-to-end check from item name to resulting `Quality` and `SellIn`. Cases include a standard item, "Aged Brie"/"AgedBrie", the full Backstage name and "Backstage", "Conjured Mana Cake"/"Conjured", and Sulfuras (quality 80, unchanged). |
| `ItemKindTests` | `ItemKind.FromName` | Maps names to the kinds `AgedBrie`, `Backstage`, `Conjured`, `Sulfuras` and `Standard`. It accepts both full and short names. Unknown names fall back to `Standard`. |
| `ItemMaturingPolicyTests` | `ItemMaturingPolicy.Mature(ItemKind, ItemState)` | Per-kind rules, using `ItemState`, `Quality` and `SellIn`. See the list below. |
| `QualityTests` | `Quality` | The constructor clamps values to 0–50. `Increase` and `Decrease` respect the bounds. `Reset` returns 0. `FromItemKind` gives `Quality.Legendary` for Sulfuras. |
| `SellInTests` | `SellIn` | `IsExpired` is true only for strictly negative values. `IsBelow(limit)` is a strict comparison. `NextDay()` decrements by one. |

The `ItemMaturingPolicyTests` cases cover these rules:
- **Standard:** quality drops by 1 per day and by 2 once the sell-in date has passed. It never goes below 0.
- **Aged Brie:** quality rises by 1, or by 2 once expired, and is capped at 50.
- **Backstage:** quality rises by 1 above 10 days, by 2 at 10 days or fewer, and by 3 at 5 days or fewer. It drops to 0 after the concert and is capped at 50.
- **Conjured:** quality degrades by 2 per day and by 4 once expired. It never goes below 0.
- **Sulfuras:** the state is returned unchanged.

These rules match the kata description in the root `README.md`.

## Running the tests

The repository's CI workflow (`.github/workflows/build.yml`, called from `dotnet.yml`) runs the following from the repository root:

```
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

To run only this project:

```
dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj
```

`dotnet.yml` sets up .NET `9.0.x` in CI. `Directory.Build.props` sets `TargetFramework` to `net8.0`, so both runtimes are relevant to a local setup.

## Build configuration and dependencies

The project file sets `IsTestProject=true` and `IsPackable=false`, and adds a global `using Xunit`. The shared `Directory.Build.props` supplies `net8.0`, `LangVersion latest`, implicit usings, nullable reference types, and `TreatWarningsAsErrors=false`. It also supplies the Roslynator and Sonar analyzers, which are disabled in Release builds.

Package references:
- `xunit` 2.9.3
- `xunit.runner.visualstudio` 4.0.0
- `Microsoft.NET.Test.Sdk` 18.9.0
- `FluentAssertions` 8.10.0
- `coverlet.collector` 10.0.1, for code coverage
- Version updates for `Roslynator.Analyzers` (5.0.0) and `SonarAnalyzer.CSharp` (10.33.0.1635)

Project reference: `GildedRoseKata.Core`.
