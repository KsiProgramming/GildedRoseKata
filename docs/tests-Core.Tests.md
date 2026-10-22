<!-- module: tests/Core.Tests -->
# Core.Tests (GildedRoseKata.Core.Tests)

Unit-test project for `src/Core` (`GildedRoseKata.Core`), the Gilded Rose kata implementation. It checks the "proposed" (refactored) code under `src/Core/ProposedCode`. That code has a small domain model and a service that applies the daily update rules to an item.

## Role in the repository

- The solution is `GildedRoseKata.sln`. It contains `src/Core` (the library) and `tests/Core.Tests` (this project).
- The tests reference the library through a `ProjectReference` to `..\..\src\Core\GildedRoseKata.Core.csproj`.
- The test classes use the namespace `GildedRoseKata.ProposedCode.Tests`. The folder layout mirrors `src/Core/ProposedCode`.
- `ItemMaturationServiceTests` also uses `GildedRoseKata.OriginalCode.Item`, the kata's `Item` class, as input. It does not test the legacy `UpdateQualityService`.

## Test files

| File | Subject under test | What it checks |
|---|---|---|
| `ProposedCode/ItemMaturationServiceTests.cs` | `ItemMaturationService.Mature(Item)` | End-to-end results for an `Item`, resolved by name. Covers a standard item, Aged Brie (`"Aged Brie"` and `"AgedBrie"`), Backstage passes (full name and `"Backstage"`), Conjured (`"Conjured Mana Cake"` and `"Conjured"`) and Sulfuras. |
| `ProposedCode/Domain/ItemKindTests.cs` | `ItemKind.FromName` | Names map to the nested kinds `AgedBrie`, `Backstage`, `Conjured`, `Sulfuras`. Any other name maps to `Standard`. |
| `ProposedCode/Domain/ItemMaturingPolicyTests.cs` | `ItemMaturingPolicy.Mature(ItemKind, ItemState)` | The per-kind rules, with boundary cases (see below). |
| `ProposedCode/Domain/QualityTests.cs` | `Quality` | The constructor clamps values into 0–50. `Increase` stops at the maximum and `Decrease` stops at the minimum. `Reset` returns 0. `FromItemKind` gives `Quality.Legendary` for Sulfuras. |
| `ProposedCode/Domain/SellInTests.cs` | `SellIn` | `IsExpired` is true only when the value is below 0. `IsBelow(limit)` is a strict comparison. `NextDay()` decrements by 1. |

### Rules the tests pin down

These come from the `InlineData` cases in `ItemMaturingPolicyTests` and `ItemMaturationServiceTests`:

- **Standard:** quality drops by 1 per day. It drops by 2 once `SellIn` is below 0, and never goes below 0. `SellIn` drops by 1.
- **Aged Brie:** quality rises by 1, or by 2 after expiry, capped at 50.
- **Backstage:** quality rises by 1 with more than 10 days left, by 2 with 10 days or fewer, and by 3 with 5 days or fewer. It is capped at 50 and drops to 0 once the concert has passed, that is when `SellIn` is 0 before the update.
- **Conjured:** quality drops by 2 per day, and by 4 once the sell-by date has passed (for example, quality 5 with `SellIn` 0 gives 1). It never goes below 0.
- **Sulfuras:** state is unchanged. `Quality` is 80 and `SellIn` stays the same.

## Dependencies

Defined in `GildedRoseKata.Core.Tests.csproj`:

- xUnit 2.9.3, with `xunit.runner.visualstudio` 4.0.0. `Xunit` is added as a global using.
- FluentAssertions 8.10.0, used for all assertions.
- Microsoft.NET.Test.Sdk 18.9.0.
- coverlet.collector 10.0.1, for code coverage.
- The project reference to `GildedRoseKata.Core`.

The root `Directory.Build.props` sets `net8.0`, nullable and implicit usings. It also adds the Roslynator and SonarAnalyzer analyzers. This project updates their versions to Roslynator.Analyzers 5.0.0 and SonarAnalyzer.CSharp 10.33.0.1635. The project is marked `IsTestProject` and is not packable.

## Running the tests

From the repository root, the same commands the CI workflow (`.github/workflows/build.yml`) uses:

```
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

To run only this project, use `dotnet test tests/Core.Tests`. The CI workflow `dotnet.yml` runs on pushes and pull requests to `master`. It sets up .NET `9.0.x`.

## Conventions

- Tests are xUnit `[Theory]` tests with `[InlineData]`. Names follow `Method_Condition_ExpectedBehavior`.
- Each file starts with a copyright header for KsiProgramming.
