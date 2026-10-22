<!-- module: tests/Core.Tests -->
# GildedRoseKata.Core.Tests

xUnit test project for `src/Core` (`GildedRoseKata.Core`). It checks the "ProposedCode" implementation of the Gilded Rose kata. That implementation is a domain model made of `ItemKind`, `Quality`, `SellIn`, `ItemState`, `ItemMaturingPolicy` and `ItemMaturationService`. The tests do not cover the `OriginalCode` namespace directly, apart from using its `Item` type.

## Role in the repository

- The project is `tests/Core.Tests/GildedRoseKata.Core.Tests.csproj`. It is part of `GildedRoseKata.sln`.
- It has a single `ProjectReference` to `..\..\src\Core\GildedRoseKata.Core.csproj`.
- `Directory.Build.props` at the repository root applies to it. That file sets `net8.0`, nullable and implicit usings enabled, `LangVersion latest`, and the Roslynator and Sonar analyzers. The analyzers are switched off in Release builds.
- The test namespace is `GildedRoseKata.ProposedCode.Tests`.

## Test files

All files are under `ProposedCode/`.

| File | Subject | What it verifies |
|---|---|---|
| `ItemMaturationServiceTests.cs` | `ItemMaturationService.Mature(Item)` | End-to-end behaviour on an `OriginalCode.Item`. Names are resolved to a kind, and the returned item has the expected `Quality` and `SellIn`. Example cases: a standard item, "Aged Brie" / "AgedBrie", full and short backstage names, "Conjured" items, and Sulfuras. |
| `Domain/ItemKindTests.cs` | `ItemKind.FromName` | Maps names to `ItemKind.AgedBrie`, `Backstage`, `Conjured`, `Sulfuras` or `Standard`. Both full names and short forms such as "Backstage" and "Conjured" are covered. Any other name maps to `Standard`. |
| `Domain/ItemMaturingPolicyTests.cs` | `ItemMaturingPolicy.Mature(kind, ItemState)` | Per-kind rules, covered in the next section. |
| `Domain/QualityTests.cs` | `Quality` | See below. |
| `Domain/SellInTests.cs` | `SellIn` | `IsExpired` is true only for strictly negative values (0 is not expired). `IsBelow(limit)` is true only when the value is strictly below the limit. `NextDay()` decrements by 1. |

### Rules covered by `ItemMaturingPolicyTests`

- **Standard:** quality drops by 1 and never goes below 0. After the sell-by date has passed (`sellIn` ≤ 0 before maturing), quality drops by 2.
- **Aged Brie:** quality rises by 1, or by 2 once past the sell-by date. It is capped at 50.
- **Backstage:** quality rises by 1 when `sellIn` is above 10, by 2 when it is 6–10, and by 3 when it is 1–5. It is capped at 50 and drops to 0 when `sellIn` is 0 before maturing.
- **Conjured:** quality drops by 2, or by 4 once past the sell-by date. It never goes below 0.
- **Sulfuras:** the state is returned unchanged. It is created with `ItemState.Create(new ItemKind.Sulfuras(), 80, 0)`.

### `QualityTests`

- The constructor clamps values into the range 0–50.
- `Increase` respects the maximum.
- `Decrease` respects the minimum.
- `Reset` returns 0.
- `Quality.FromItemKind(new ItemKind.Sulfuras(), 12)` yields `Quality.Legendary`.

## Dependencies

- xunit 2.9.3 and `xunit.runner.visualstudio` 4.0.0
- Microsoft.NET.Test.Sdk 18.9.0
- FluentAssertions 8.10.0, used for all assertions
- coverlet.collector 10.0.1, for code coverage
- The project overrides the analyzer package versions for `Roslynator.Analyzers` (5.0.0) and `SonarAnalyzer.CSharp` (10.33.0.1635).
- `Xunit` is added as a global using.

## Running the tests

From the repository root:

```
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

These are the same steps as the CI workflow (`.github/workflows/build.yml`, called from `dotnet.yml` on pushes and pull requests to `master`). CI uses .NET SDK 9.0.x. To run only this project, use `dotnet test tests/Core.Tests`.

## Conventions

- Tests are mostly `[Theory]` methods with `[InlineData]`. They are named `Method_Condition_ExpectedBehaviour`.
- Source files carry a copyright header (KsiProgramming).
