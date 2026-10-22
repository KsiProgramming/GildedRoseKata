<!-- module: tests/Core.Tests -->
# GildedRoseKata.Core.Tests

## Purpose

`tests/Core.Tests` is the xUnit test project for `src/Core` (`GildedRoseKata.Core`). It checks the refactored "ProposedCode" implementation of the Gilded Rose kata: the domain value types, the item-kind resolution, the maturing rules and the orchestrating service.

The project has no tests for `OriginalCode`. That namespace appears only as the `Item` data type that the service accepts and returns.

The user asked for a full arc42/C4 Software Architecture Document. The repository holds only a small test project. It shows nothing about cloud infrastructure, deployment, regulatory constraints, ADRs or runtime topology, so this page leaves those out. It covers the sections the files support.

## Context in the repository

```text
GildedRoseKata.sln
├── src/Core/                      GildedRoseKata.Core (system under test)
│   ├── OriginalCode/              Item, UpdateQualityService
│   └── ProposedCode/              ItemMaturationService, IMaturingPolicy, Domain/*
└── tests/Core.Tests/              GildedRoseKata.Core.Tests (this module)
    └── ProposedCode/              tests mirroring src/Core/ProposedCode
```

- The test project references `..\..\src\Core\GildedRoseKata.Core.csproj` through a `ProjectReference`.
- The folder layout under `ProposedCode/` mirrors the source layout.
- All test classes use the namespace `GildedRoseKata.ProposedCode.Tests`.
- `Directory.Build.props` at the repository root applies to the project. It sets `net8.0`, `LangVersion latest`, implicit usings, nullable reference types and `TreatWarningsAsErrors=false`. It also adds the Roslynator and SonarAnalyzer analyzers, and analyzers are switched off in Release.
- The test csproj overrides the analyzer package versions with `PackageReference Update`: Roslynator.Analyzers 5.0.0 and SonarAnalyzer.CSharp 10.33.0.1635.

## Test suites

| File | Subject | What it verifies |
|---|---|---|
| `ProposedCode/Domain/ItemKindTests.cs` | `ItemKind.FromName` | Maps item names to the kinds `AgedBrie`, `Backstage`, `Conjured`, `Sulfuras` and `Standard`. Both full names and short names resolve. Examples are `"Aged Brie"`/`"AgedBrie"`, the full TAFKAL80ETC backstage pass name/`"Backstage"`, and `"Conjured Mana Cake"`/`"Conjured"`. `"Sulfuras, Hand of Ragnaros"` resolves to `Sulfuras`. Unrecognised names such as `"Elixir of the Mongoose"` resolve to `Standard`. |
| `ProposedCode/Domain/ItemMaturingPolicyTests.cs` | `ItemMaturingPolicy.Mature(kind, state)` | Per-kind rules, using `ItemState(new Quality(q), new SellIn(s))`. |
| `ProposedCode/Domain/QualityTests.cs` | `Quality` | The constructor clamps values to 0–50. `Increase` caps at the maximum and `Decrease` floors at the minimum. `Reset` returns 0. `Quality.FromItemKind(new ItemKind.Sulfuras(), …)` returns `Quality.Legendary`. |
| `ProposedCode/Domain/SellInTests.cs` | `SellIn` | `IsExpired` is true only for strictly negative values (0 is not expired). `IsBelow(limit)` is a strict comparison. `NextDay()` decrements by one. |
| `ProposedCode/ItemMaturationServiceTests.cs` | `ItemMaturationService.Mature(Item)` | An end-to-end check through the default policy. An `OriginalCode.Item` goes in and a matured `Item` comes out, for each item kind and name variant. |

### Rules the maturing-policy tests encode

The cases are `[Theory]`/`[InlineData]` rows of the form `(quality, sellIn) → (expectedQuality, expectedSellIn)`.

- **Standard**
  - Quality drops by 1 per day, and by 2 once expired. For example, `(2, 0) → (0, -1)`.
  - Quality never goes below 0.
- **Aged Brie**
  - Quality rises by 1, and by 2 once expired, for example `(20, 0) → (22, -1)`.
  - Quality is capped at 50.
- **Backstage**
  - Quality rises by 1 when SellIn is above 10, by 2 when SellIn is 10 to 6, and by 3 when SellIn is 5 to 1.
  - At SellIn 0 the quality drops to 0, so `(20, 0) → (0, -1)`.
  - Quality is capped at 50.
- **Conjured**
  - Quality drops by 2 per day, and by 4 once expired, for example `(5, 0) → (1, -1)`.
  - Quality never goes below 0.
- **Sulfuras**
  - The state is returned unchanged. `ItemState.Create(new ItemKind.Sulfuras(), 80, 0)` stays at quality 80 and SellIn 0.
  - The service-level test also expects `(80, 0) → (80, 0)`.

These rules match the kata description in the root `README.md`.

## Conventions

- **Framework:** xUnit with `[Fact]` and `[Theory]`/`[InlineData]`. `Xunit` is added as a global `<Using>`.
- **Assertions:** FluentAssertions, for example `result.Value.Should().Be(expected)` and `Should().BeOfType(...)`.
- **Naming:** `Method_WhenCondition_ExpectedBehavior`, for example `Mature_WhenKindIsStandard_FollowsExpectedRules`.
- **File headers:** each file starts with a copyright banner for "KsiProgramming".
- **Indentation:** some files use tabs (`QualityTests`, `SellInTests`) and others use spaces. This is an inconsistency in the files.

## Dependencies

| Package | Version | Role |
|---|---|---|
| `Microsoft.NET.Test.Sdk` | 18.9.0 | Test host |
| `xunit` | 2.9.3 | Test framework |
| `xunit.runner.visualstudio` | 4.0.0 | Test discovery and runner adapter |
| `FluentAssertions` | 8.10.0 | Assertion library |
| `coverlet.collector` | 10.0.1 | Code-coverage collector |
| `Roslynator.Analyzers`, `SonarAnalyzer.CSharp` | 5.0.0 and 10.33.0.1635 | Analyzers (versions overridden here) |
| `GildedRoseKata.Core` (project) | – | System under test |

The csproj sets `IsPackable=false` and `IsTestProject=true`.

## Build and run

From the repository root, the CI workflow runs these commands:

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

To run only this project, use `dotnet test tests/Core.Tests`. Coverage collection is available through `coverlet.collector`, for example `dotnet test --collect:"XPlat Code Coverage"`.

### Continuous integration

- `.github/workflows/dotnet.yml` runs on pushes and pull requests to `master`. Pull requests that change only `.gitignore` or `README.md` are skipped.
- It calls the reusable workflow `.github/workflows/build.yml` on `ubuntu-latest` with `dotnet-version: 9.0.x`.
- `build.yml` runs checkout, `setup-dotnet`, restore, build and test.
- The projects target `net8.0` while CI installs the .NET 9 SDK. The files do not show how a .NET 8 runtime is made available for running the tests.

## Notes for maintainers

- When you add behaviour under `src/Core/ProposedCode`, add tests under the matching folder in `ProposedCode/` and use the same namespace.
- `ItemMaturationServiceTests` uses the parameterless `ItemMaturationService` constructor, which wires in `ItemMaturingPolicy`. The service also has a constructor that takes an `IMaturingPolicy`. No test uses it yet, for example to inject a fake policy.
- No tests exist for `OriginalCode/UpdateQualityService`, so nothing compares the legacy and proposed implementations.
