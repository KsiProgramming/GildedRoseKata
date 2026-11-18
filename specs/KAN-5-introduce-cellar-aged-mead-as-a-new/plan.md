# Implementation plan: Introduce Cellar-Aged Mead as a new maturing product (KAN-5)

## Technical context
- **Language and framework:** C# (latest `LangVersion`), `net8.0` set in `Directory.Build.props`. Nullable and implicit usings are on. CI (`.github/workflows/dotnet.yml` → `build.yml`) uses .NET SDK 9.0.x.
- **Tests:** xunit 2.9.3 with FluentAssertions 8.x in `tests/Core.Tests`. The tests use `[Theory]`/`[InlineData]` and are named `Method_WhenCondition_Expected`.
- **Analyzers:** Roslynator and SonarAnalyzer run in Debug. Warnings are not treated as errors.
- **Conventions:**
  - Each file starts with the copyright header block.
  - Members are accessed with `this.`.
  - Namespace is `GildedRoseKata.ProposedCode`.
  - Test namespace is `GildedRoseKata.ProposedCode.Tests`.
- **Architecture of the update path (`src/Core/ProposedCode`):**
  - `ItemMaturationService.Mature(Item)` calls `ItemKind.FromName(name)`.
  - It then builds an `ItemState` (`Quality` clamped to 0–50, `SellIn`) and calls `IMaturingPolicy.Mature(kind, state)`.
  - `ItemMaturingPolicy` switches on the kind and has one private method per kind.
  - The service returns a new `Item`, so each item is processed independently and without side effects.
- **Legacy path:** `OriginalCode/UpdateQualityService` is the original kata code. Nothing references it. `Item` is in `OriginalCode` and must not change (out of scope).

## Approach
1. **Product identity (FR-001).** Add a sealed record `ItemKind.CellarAgedMead` and one exact-match arm in `ItemKind.FromName`: `"Cellar-Aged Mead" => new CellarAgedMead()`.
   - The switch uses exact, case-sensitive string patterns, like the existing names. This resolves the first open question.
   - There is no substring or `Contains` matching, so names such as "Mead" or "Cellar-Aged Mead Reserve" stay `Standard` (AC-012).
2. **Maturation rule (FR-002, FR-003, FR-004, FR-005, FR-006).** Add an arm `ItemKind.CellarAgedMead => this.MatureCellarAgedMead(state)` in `ItemMaturingPolicy.Mature`. Add a private method that follows the existing per-kind pattern:
   - `newQuality = state.Quality.Increase()`, `newSellIn = state.SellIn.NextDay()`.
   - If `newSellIn.IsExpired`, then `newQuality = newQuality.Increase()`.
   - Return `new(newQuality, newSellIn)`.
   - `IsExpired` is `Value < 0` on the new sell-in. That equals the spec's "old sell-in ≤ 0 → +2, otherwise +1".
   - The cap at 50 and the floor at 0 come from `Quality` (clamping in its constructor). Nothing is duplicated.
   - Quality never decreases, so an expired mead keeps gaining.
   - The method is deliberately separate from `MatureAgedBrie`, even though the arithmetic is the same today. That keeps the new behaviour isolated, as the spec requires, and lets the two products change independently later.
3. **Independence and determinism (FR-008, FR-009).** No change is needed. The policy is a pure function of `(kind, state)` and the service builds a new `Item`.
4. **Existing products (FR-007).** The existing arms and methods are untouched. The new switch arm sits before the `_` default.
5. **Legacy `UpdateQualityService` (open question 2).** Leave it unchanged. It is the kata's legacy implementation, nothing calls it, and the spec says to avoid changing existing logic. `ProposedCode` is the update path that gets the feature. If the approver wants the legacy path covered too, that is a separate decision, because the nested-`if` code is risky to extend.
6. **Docs.** Add a short note on the mead rule to `README.md` under the kata description. This covers the "update documentation" subtask and is optional.

## Files
- `src/Core/ProposedCode/Domain/ItemKind.cs` (change): add the `CellarAgedMead` record and the exact-name mapping.
- `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` (change): add the switch arm and `MatureCellarAgedMead`.
- `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` (change): add the name mapping, plus negative cases for a partial name, different case and trailing whitespace (all `Standard`).
- `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` (change): add a theory for the mead rules and boundaries, plus repeated-update, determinism and negative sell-in tests.
- `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` (change): add mead rows to the existing theory. Add tests for several mead entries mixed with other products, and a regression test over all existing kinds.
- `README.md` (change, optional): document the new product.
- Not changed: `Item.cs`, `UpdateQualityService.cs`, `IMaturingPolicy.cs`, `ItemMaturationService.cs`, `Quality.cs`, `SellIn.cs`, `ItemState.cs`.

## Risks
- **Shared switch in `ItemMaturingPolicy`.** A mistake in the arm order could reroute other kinds. The new arm goes before `_` and is a distinct type, so the risk is low. The regression tests cover it.
- **Public interface.** `ItemKind` is a public abstract record. Adding a nested sealed record is additive. A caller with an exhaustive switch over kinds would hit the default branch, and none exists in the repo.
- **`Quality` clamping.** A mead with an initial quality outside 0–50 is clamped on `ItemState.Create`. This is existing behaviour and satisfies FR-010 for 0–50. Quality above 50 is not a supported mead input.
- **Name matching.** Exact matching means any variant spelling (case or whitespace) is a Standard item that loses quality. This is intended per FR-001, and the tests pin it.
- **Legacy path.** The mead is not handled by `OriginalCode.UpdateQualityService`. This is documented as an assumption.
- **Migrations and config:** none.

## Test plan
Run all tests with `dotnet test` from the repo root. CI runs `dotnet restore`, `dotnet build --no-restore` and `dotnet test --no-build`. To run a subset: `dotnet test --filter "FullyQualifiedName~GildedRoseKata.ProposedCode.Tests"`.

| Requirement | Proving tests |
|---|---|
| FR-001 (AC-012) | `ItemKindTests`: "Cellar-Aged Mead" → `CellarAgedMead`. "Cellar-Aged", "Mead", "Cellar-Aged Mead Reserve", "cellar-aged mead" and "Cellar-Aged Mead " → `Standard`. `ItemMaturationServiceTests`: a mead row. |
| FR-002 (AC-001, AC-006) | Policy and service theory rows: (20, 5) → (21, 4); (20, 1) → (21, 0). |
| FR-003 (AC-002, AC-003) | Rows: (20, 0) → (22, -1); (30, -5) → (32, -6); (20, -1) → (22, -2). |
| FR-004 (AC-004, AC-005, AC-008) | Rows: (49, 5) → 50; (50, 0) → 50 with sell-in -1; (49, 0) → 50; (48, 0) → 50; (48, 5) → 49; (50, 5) → 50. |
| FR-005 | Every row asserts sell-in decreases by exactly 1, including negative sell-in and quality 50. |
| FR-006 (AC-013) | Negative sell-in rows assert quality ≥ the start value, never lower. |
| FR-007 (AC-007) | The existing policy and service theories stay unchanged. Add a mixed-inventory test with Standard, Aged Brie, Backstage, Conjured and Sulfuras items, asserting the known pre-change outputs. |
| FR-008 (AC-009) | A service test with several mead entries at different sell-in and quality values plus other products. Each result must equal the result of updating that item alone, and the input items must not be mutated. |
| FR-009 (AC-010, SC-002) | Repeated updates: (20, 2) over 3 days → (24, -1). Cumulative runs from 0 and from 48 are capped at 50. Running twice from the same values gives equal results. |
| FR-010 (AC-011, SC-003) | Rows with quality 0 → 1 (sell-in > 0) and 2 (sell-in ≤ 0), and 48 → 49 and 50. A long loop asserts quality stays within 0–50. |

The SC-004 existing-suite check is `dotnet test` with the existing expected values unchanged.