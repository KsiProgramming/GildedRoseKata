<!-- module: tests/Core.Tests -->
# Core.Tests (GildedRoseKata.Core.Tests)

Unit-test project for the `GildedRoseKata.Core` library in `src/Core`. It tests the "ProposedCode" refactoring of the Gilded Rose kata: the domain types and the service that matures items each day.

## Role in the repository

- The solution is `GildedRoseKata.sln`. It contains `src/Core` (the library) and this test project.
- `src/Core` has two parts: `OriginalCode/` (`Item`, `UpdateQualityService`) and `ProposedCode/` (the domain model and policy).
- The tests cover only `ProposedCode`. The test folder layout mirrors the library: `ProposedCode/` and `ProposedCode/Domain/`.
- `ItemMaturationServiceTests` uses `GildedRoseKata.OriginalCode.Item` as its input type. No test targets `UpdateQualityService` directly.

## Test files

| File | Subject | What it checks |
|---|---|---|
| `ProposedCode/Domain/ItemKindTests.cs` | `ItemKind.FromName` | Item names map to the expected kind: `Aged Brie` and `AgedBrie` give `AgedBrie`. `Backstage passes to a TAFKAL80ETC concert` and `Backstage` give `Backstage`. `Conjured Mana Cake` and `Conjured` give `Conjured`. `Sulfuras, Hand of Ragnaros` gives `Sulfuras`. Any other name, such as `Elixir of the Mongoose`, gives `Standard`. |
| `ProposedCode/Domain/ItemMaturingPolicyTests.cs` | `ItemMaturingPolicy.Mature(kind, ItemState)` | Quality and sell-in rules for each kind (see below). |
| `ProposedCode/Domain/QualityTests.cs` | `Quality` | The constructor clamps values to 0–50. `Increase` stops at the maximum and `Decrease` stops at the minimum. `Reset` returns 0. `Quality.FromItemKind` returns `Quality.Legendary` for Sulfuras. |
| `ProposedCode/Domain/SellInTests.cs` | `SellIn` | `IsExpired` is true only when the value is below zero. `IsBelow(limit)` is a strict comparison. `NextDay()` decrements by one. |
| `ProposedCode/ItemMaturationServiceTests.cs` | `ItemMaturationService.Mature(Item)` | End-to-end check that an item resolves to the right kind and gets the right rules applied. It returns an object exposing `Quality` and `SellIn`. |

### Rules covered by `ItemMaturingPolicyTests`

- **Standard:** quality drops by 1 and sell-in drops by 1. Quality never goes below 0. Once sell-in is 0 or less, quality drops by 2.
- **Aged Brie:** quality rises by 1, or by 2 once sell-in is 0 or less. It is capped at 50.
- **Backstage:**
  - Quality rises by 1 when sell-in is above 10.
  - It rises by 2 when sell-in is 10 down to 6.
  - It rises by 3 when sell-in is 5 down to 1.
  - It is capped at 50.
  - Quality drops to 0 when sell-in is 0 and the concert has passed.
- **Conjured:** quality drops by 2, or by 4 once sell-in is 0 or less. It is floored at 0.
- **Sulfuras:** the state is returned unchanged. The tests build it with `ItemState.Create(new ItemKind.Sulfuras(), 80, 0)`.

## Tooling and dependencies

From `GildedRoseKata.Core.Tests.csproj`:

- Project reference: `..\..\src\Core\GildedRoseKata.Core.csproj`.
- Test packages:
  - xUnit 2.9.3
  - `xunit.runner.visualstudio` 4.0.0
  - `Microsoft.NET.Test.Sdk` 18.9.0
  - FluentAssertions 8.10.0
  - `coverlet.collector` 10.0.1 for coverage
- `Xunit` is added as a global `Using`.
- The project sets `IsTestProject=true` and `IsPackable=false`.
- Roslynator and SonarAnalyzer package versions are updated in this project. The root `Directory.Build.props` adds both analyzers to every project and turns analyzers off in Release.
- `Directory.Build.props` sets `net8.0`, nullable and implicit usings enabled, `LangVersion` latest, and `TreatWarningsAsErrors=false`.

Tests are `[Theory]`/`[InlineData]` cases with FluentAssertions assertions. Test classes sit in the namespace `GildedRoseKata.ProposedCode.Tests`.

## Running the tests

From the repository root:

```
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

These are the commands the CI workflow (`.github/workflows/build.yml`, called from `dotnet.yml`) runs on Ubuntu. It uses .NET SDK 9.0.x and triggers on pushes and pull requests to `master`.

To run only this project, use `dotnet test tests/Core.Tests`.
