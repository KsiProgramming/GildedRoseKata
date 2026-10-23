<!-- module: tests/Core.Tests -->
# GildedRoseKata.Core.Tests

This is the xUnit test project for `src/Core` (`GildedRoseKata.Core`) in the Gilded Rose kata solution. It tests the "ProposedCode" refactoring of the kata. That code is made up of a small domain model and a maturation service.

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

The test classes use the namespace `GildedRoseKata.ProposedCode.Tests`.

## What is tested

All tests are data-driven `[Theory]`/`[InlineData]` cases, apart from a few `[Fact]` cases. They use FluentAssertions.

| Test class | Subject | What it checks |
|---|---|---|
| `ItemMaturationServiceTests` | `ItemMaturationService.Mature(Item)` | This is an end-to-end check. It takes an `OriginalCode.Item` with a name, quality and sell-in, and checks the resulting `Quality` and `SellIn`. The cases cover a standard item ("Elixir of the Mongoose"), Aged Brie, Backstage passes, Conjured items and Sulfuras. They include short names such as `AgedBrie`, `Backstage` and `Conjured`. |
| `ItemKindTests` | `ItemKind.FromName` | Maps item names to the kinds `AgedBrie`, `Backstage`, `Conjured`, `Sulfuras` and `Standard`. Both full names and short names are covered. |
| `ItemMaturingPolicyTests` | `ItemMaturingPolicy.Mature(ItemKind, ItemState)` | Covers the per-kind rules at their boundaries. Standard items degrade and degrade faster after the sell-by date. Aged Brie improves up to a quality cap of 50. Backstage passes gain more quality as the concert nears, with thresholds around sell-in 10 and 5, and drop to 0 after the concert. Conjured items degrade at double speed. Sulfuras keeps its state unchanged. |
| `QualityTests` | `Quality` value type | The constructor clamps values to the range 0–50. `Increase` and `Decrease` respect the maximum and minimum. `Reset` returns 0. `FromItemKind` returns `Quality.Legendary` for Sulfuras. |
| `SellInTests` | `SellIn` value type | `IsExpired` is true only for strictly negative values. `IsBelow(limit)` checks the value against a limit. `NextDay()` decrements by one. |

## Running the tests

The repository shows no dedicated test script. The project is a standard SDK-style test project, so you can run it with the .NET CLI:

```
dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj
```

You can also run `dotnet test` against `GildedRoseKata.sln` at the repository root. The CI workflow `.github/workflows/dotnet.yml` runs on pushes and pull requests to `master`. It calls the reusable `build.yml` workflow with .NET `9.0.x`.

## Dependencies

- Project reference: `src/Core/GildedRoseKata.Core.csproj`.
- Packages (from the `.csproj`):
  - `xunit` 2.9.3
  - `xunit.runner.visualstudio` 4.0.0
  - `Microsoft.NET.Test.Sdk` 18.9.0
  - `FluentAssertions` 8.10.0
  - `coverlet.collector` 10.0.1, for code coverage
- `Xunit` is added as a global `Using`. The project sets `IsPackable=false` and `IsTestProject=true`.
- The root `Directory.Build.props` sets `net8.0` as the target framework. It also sets the latest C# language version, and enables implicit usings and nullable reference types. `Roslynator.Analyzers` and `SonarAnalyzer.CSharp` run as analyzers in Debug builds. The test project overrides their versions to 5.0.0 and 10.33.0.1635. Analyzers are disabled in Release builds.
