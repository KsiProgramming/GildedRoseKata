# Implementation plan: Introduce a new maturing product (Vintage Wine)

Issue: KAN-1. Spec: `spec.md`.

## Technical context

- **Language and runtime:** C# with `LangVersion` latest, nullable and implicit usings enabled. `Directory.Build.props` targets `net8.0`. CI (`dotnet.yml` calling `build.yml`) installs .NET SDK 9.0.x.
- **Solution layout:** `GildedRoseKata.sln` holds `src/Core` (`GildedRoseKata.Core.csproj`) and `tests/Core.Tests` (`GildedRoseKata.Core.Tests.csproj`).
- **Test tools:** xUnit 2.9.3, FluentAssertions 8.10.0 and coverlet. Tests are `[Theory]`/`[InlineData]` methods named `Method_WhenX_DoesY`.
- **Static analysis:** Roslynator and SonarAnalyzer run as analyzers. `TreatWarningsAsErrors` is false, but SC-003 requires no new warnings.
- **CI:** `dotnet restore`, then `dotnet build --no-restore`, then `dotnet test --no-build`.
- **Code style:** every file starts with the copyright header block. Types use file-scoped namespaces and `this.` member access.
- **Scope:** only the `ProposedCode` maturing flow changes. The flow is `ItemMaturationService.Mature`, which calls `ItemKind.FromName`, then `ItemState.Create`, then `IMaturingPolicy.Mature`. `OriginalCode` stays untouched.

## Approach

Follow the pattern the existing kinds use: a nested sealed record in `ItemKind`, a name mapping in `FromName`, and a private `MatureX` method selected by the policy's `switch`.

1. **`ItemKind`**
   - Add `public sealed record VintageWine : ItemKind;`.
   - Add the mappings `"Vintage Wine"` and `"Vintage"` to `VintageWine` in `FromName`. Matching stays exact and case-sensitive, as the spec requires (FR-001).
2. **`ItemMaturingPolicy`**
   - Add the arm `ItemKind.VintageWine => this.MatureVintageWine(state)` before the default arm.
   - Add `MatureVintageWine`, which reads SellIn at the start of the day, as `MatureBackstage` does:
     - `state.SellIn.IsBelow(11)` gives `state.Quality.Increase(2)` (SellIn ≤ 10, including 0 and negative values, FR-004).
     - Otherwise it gives `state.Quality.Increase()` (SellIn ≥ 11, FR-003).
     - SellIn becomes `state.SellIn.NextDay()` (FR-002).
   - No expiry penalty is applied, so Quality never drops (FR-005).
   - `Quality.Increase` already clamps to 50 (FR-006).
3. **`ItemMaturationService`**
   - No change. It already preserves `item.Name` (FR-008) and routes through `ItemKind.FromName` and the policy.
4. **`Quality`, `SellIn`, `ItemState`, `IMaturingPolicy`**
   - No change. `Quality.FromItemKind` only special-cases Sulfuras, so Vintage Wine is clamped to 0–50 on creation.
5. **Existing kinds**
   - Their code paths are not edited, so they behave as before (FR-007).

## Files

| File | Change | Why |
|---|---|---|
| `src/Core/ProposedCode/Domain/ItemKind.cs` | Change | Add the `VintageWine` record and the two name mappings (FR-001). |
| `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` | Change | Add the switch arm and `MatureVintageWine` (FR-002 to FR-006). |
| `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` | Change | Add `InlineData` rows for "Vintage Wine" and "Vintage" (AC-001). |
| `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` | Change | Add a `Mature_WhenKindIsVintageWine_FollowsExpectedRules` theory (AC-002 to AC-006). |
| `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` | Change | Add Vintage Wine rows to the existing theory. Add one test that the name is preserved (AC-008, SC-004). |

No new files, packages, project-file edits or CI edits are needed.

## Risks

- **Shared switch in `ItemMaturingPolicy`:** the default arm is `MatureStandard`. The new arm must sit before `_`, otherwise Vintage Wine would be treated as standard. Existing arms are not edited.
- **`ItemKind.FromName`:** the new strings must not collide with existing ones. They don't.
- **Public interface:** `IMaturingPolicy` is unchanged. Adding a nested record to `ItemKind` is additive. Custom `IMaturingPolicy` implementations that don't handle `VintageWine` would treat it as their default, which is outside this scope.
- **Threshold off-by-one:** the boundary uses start-of-day SellIn with `IsBelow(11)`. The tests cover SellIn 11, 10, 1, 0 and -5 to pin it.
- **Analyzer warnings:** follow the existing method shape, with no new magic-number patterns beyond what `MatureBackstage` already uses, to avoid new Sonar or Roslynator warnings.
- **No migrations or data changes.**

## Test plan

Run all tests with `dotnet test` from the repository root. CI runs `dotnet restore`, `dotnet build --no-restore`, then `dotnet test --no-build`. To run only the affected tests, use `dotnet test --filter "FullyQualifiedName~ProposedCode.Tests"`.

| Requirement | Proving test |
|---|---|
| FR-001 / AC-001 | `ItemKindTests.FromName_MapsNamesToExpectedKinds`: "Vintage Wine" and "Vintage" give `ItemKind.VintageWine`. |
| FR-002 / AC-002 | `ItemMaturingPolicyTests` Vintage Wine theory: expected SellIn is the input minus 1 for SellIn 20, 11, 10, 5, 1, 0 and -5. |
| FR-003 / AC-003 | Same theory: (Q10, SellIn 20) gives Q11 and (Q10, SellIn 11) gives Q11. |
| FR-004 / AC-004 | Same theory: (Q10, SellIn 10, 1, 0 or -5) gives Q12. |
| FR-005 / AC-005 | Same theory: (Q10, SellIn -5) gives Q12, which is at least 10. |
| FR-006 / AC-006 | Same theory: (Q50, SellIn 20) gives Q50, (Q49, SellIn 5) gives Q50, and (Q49, SellIn 20) gives Q50. |
| FR-007 / AC-007 / SC-002 | The existing Standard, AgedBrie, Backstage, Conjured and Sulfuras tests in `ItemMaturingPolicyTests` and `ItemMaturationServiceTests` pass with no expectation changes. |
| FR-008 / AC-008 | `ItemMaturationServiceTests`: a new `[Fact]` asserts `result.Name == "Vintage Wine"` after `Mature`. |
| SC-001 | The policy theory above covers the boundaries at SellIn 11, 10 and 0, expired items, and the Quality cap of 50. |
| SC-004 | New rows in `ItemMaturationServiceTests.Mature_UsesExpectedRulesForResolvedItemKinds`: ("Vintage Wine", 10, 20 → 11, 19), (10, 11 → 11, 10), (10, 10 → 12, 9), (10, 1 → 12, 0), (10, 0 → 12, -1), (10, -5 → 12, -6), (50, 20 → 50, 19), (49, 5 → 50, 4). One row also uses the "Vintage" short name. |
| SC-003 | `dotnet build` produces no new warnings, and the CI workflow stays green. |