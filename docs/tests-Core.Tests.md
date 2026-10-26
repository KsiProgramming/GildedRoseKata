<!-- module: tests/Core.Tests -->
# GildedRoseKata.Core.Tests

This is the xUnit test project for `src/Core` (`GildedRoseKata.Core`). It tests the "ProposedCode" part of the Gilded Rose kata: the refactored domain model and the maturation service. The project is not packable (`IsPackable=false`) and is marked as a test project (`IsTestProject=true`).

## How it fits in the repository

- The solution is `GildedRoseKata.sln`. The only production project is `src/Core`, which has two code areas:
  - `OriginalCode/` contains `Item` and `UpdateQualityService`.
  - `ProposedCode/` contains `ItemMaturationService`, `IMaturingPolicy`, and `Domain/` (`ItemKind`, `ItemMaturingPolicy`, `ItemState`, `Quality`, `SellIn`).
- This project references `..\..\src\Core\GildedRoseKata.Core.csproj`.
- The test files sit under `ProposedCode/`, which mirrors the layout of `src/Core/ProposedCode`.
- Shared build settings come from the root `Directory.Build.props`. They include `net8.0`, nullable and implicit usings enabled, and the latest C# language version. The Roslynator and SonarAnalyzer analyzers are also referenced there. This project overrides their versions to 5.0.0 and 10.33.0.1635.

## Test files

All tests use the namespace `GildedRoseKata.ProposedCode.Tests`. They are `[Theory]` tests with `[InlineData]` rows, plus a few `[Fact]` tests. Assertions use FluentAssertions.

| File | Subject | What it checks |
|---|---|---|
| `ProposedCode/ItemMaturationServiceTests.cs` | `ItemMaturationService.Mature(Item)` | An end-to-end test that builds an `OriginalCode.Item` from a name, quality and sell-in. It checks the resulting `Quality` and `SellIn` for a standard item, Aged Brie (also written `AgedBrie`), Backstage passes (full name and `Backstage`), Conjured items, and Sulfuras. |
| `ProposedCode/Domain/ItemKindTests.cs` | `ItemKind.FromName` | Item names map to the nested kinds `AgedBrie`, `Backstage`, `Conjured`, `Sulfuras` and `Standard`. Both full and short names are covered, for example `Conjured` and `Conjured Mana Cake`. |
| `ProposedCode/Domain/ItemMaturingPolicyTests.cs` | `ItemMaturingPolicy.Mature(kind, ItemState)` | Quality and sell-in rules for each kind. Standard items lose quality, and lose it faster once sell-in is below 0. Aged Brie gains quality, capped at 50. Backstage passes gain 1, 2 or 3 depending on how close the sell-in is, and drop to 0 after the concert. Conjured items degrade at double speed. Sulfuras returns its state unchanged. |
| `ProposedCode/Domain/QualityTests.cs` | `Quality` | The constructor clamps values to 0–50. `Increase` and `Decrease` respect those bounds. `Reset` returns 0. `FromItemKind` uses `Quality.Legendary` for Sulfuras. |
| `ProposedCode/Domain/SellInTests.cs` | `SellIn` | `IsExpired` is true only for strictly negative values. `IsBelow(limit)` and `NextDay()` (decrement by one) are also tested. |

## Running the tests

The repository shows no custom test scripts. The standard .NET commands should work from the repository root:

```
dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj
```

The CI workflow `.github/workflows/dotnet.yml` runs on pushes and pull requests to `master`. It calls the reusable workflow `build.yml` with `dotnet-version: 9.0.x`.

`coverlet.collector` is installed, so coverage can be collected with `dotnet test --collect:"XPlat Code Coverage"`.

## Dependencies

- `xunit` 2.9.3, with `xunit.runner.visualstudio` 4.0.0 as the runner. `Xunit` is a global `using`.
- `Microsoft.NET.Test.Sdk` 18.9.0
- `FluentAssertions` 8.10.0
- `coverlet.collector` 10.0.1
- Project reference: `GildedRoseKata.Core`
