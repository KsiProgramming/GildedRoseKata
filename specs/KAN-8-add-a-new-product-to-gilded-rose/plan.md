# Implementation plan: Add Mature Cheese product (KAN-8)

## Technical context
- Language and runtime: C# on .NET 9 (CI uses `dotnet-version: 9.0.x`). The solution is `GildedRoseKata.sln`.
- Source project: `src/Core/GildedRoseKata.Core.csproj`.
  - Legacy code is in `src/Core/OriginalCode` (`Item`, `UpdateQualityService`). **It must not be touched** (FR-013).
  - The new code is in `src/Core/ProposedCode`.
- Existing design in the proposed code:
  - `ItemMaturationService.Mature(Item)` calls `ItemKind.FromName(name)`.
  - It then calls `ItemState.Create(kind, quality, sellIn)`, which builds `Quality` and `SellIn`.
  - It then calls `IMaturingPolicy.Mature(kind, state)`. The implementation is `ItemMaturingPolicy`, which has one private method per kind.
  - It returns a new `Item`.
- `Quality` is a value type whose constructor clamps to 0–50. This is the existing policy for invalid legacy data. `Increase(n)` and `Decrease(n)` clamp too. Sulfuras is the only kind pinned to 80.
- `SellIn` has `NextDay()` and `IsExpired` (`Value < 0`).
- `ItemKind` is an abstract record with one nested sealed record per kind. `FromName` is an exact, case-sensitive `switch` on the name, and unknown names map to `Standard`.
- Tests: xUnit 2.9.3, FluentAssertions 8.10, coverlet. The project is `tests/Core.Tests`.
  - Test files mirror the source layout.
  - They use `[Theory]`/`[InlineData]` and `Method_WhenCondition_Expected` names.
  - The namespace is `GildedRoseKata.ProposedCode.Tests`, and files carry the copyright header.
- CI (`.github/workflows/build.yml`) runs `dotnet restore`, `dotnet build --no-restore` and `dotnet test --no-build`.

## Approach
Add Mature Cheese as a sixth `ItemKind`, with its own isolated method in `ItemMaturingPolicy`, following the existing Aged Brie pattern. No other rule is touched.

1. **`ItemKind`**
   - Add `public sealed record MatureCheese : ItemKind;`.
   - Add the arm `"Mature Cheese" => new MatureCheese()` to `FromName`.
   - Matching stays exact and case-sensitive, so `mature cheese` and `Mature Cheese Deluxe` fall through to `Standard` (FR-001, FR-009).
2. **`ItemMaturingPolicy`**
   - Add the case `ItemKind.MatureCheese => this.MatureMatureCheese(state)` to the switch. The method name can be `MatureCheese`, if that avoids the awkward repetition.
   - The rule uses the sellIn from before the decrement (FR-005):
     - `increase = state.SellIn.Value > 0 ? 1 : 2`
     - `newQuality = state.Quality.Increase(increase)`, which clamps to 50 (FR-006, FR-007)
     - `newSellIn = state.SellIn.NextDay()` (FR-002)
     - Return `new(newQuality, newSellIn)`.
   - Alternative: mirror Aged Brie with `Increase()` and then a second `Increase()` if `newSellIn.IsExpired`. This is equivalent, because `sellIn <= 0` before the update is the same as `newSellIn < 0`. The explicit "before decrement" form is clearer against the spec. To avoid adding a `SellIn` member, read `state.SellIn.Value` or use `!state.SellIn.IsBelow(1)`.
3. **Negative or invalid input quality (FR-008)**
   - `ItemState.Create` already builds `Quality` through the clamping constructor. A loaded quality of -5 becomes 0 before the rule runs, so the result is 1 or 2.
   - No change is needed in `Quality`, `SellIn`, `ItemState`, `IMaturingPolicy` or `ItemMaturationService`.
4. **FR-011 and FR-012 (no mutation outside update, determinism)**
   - `Mature` returns a new `Item` and does not modify its input. The policy is pure.
   - These requirements are satisfied by the existing design, and tests will confirm it.

## Files
- `src/Core/ProposedCode/Domain/ItemKind.cs`: add the `MatureCheese` record and the `"Mature Cheese"` mapping.
- `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs`: add the switch case and the private Mature Cheese method.
- `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs`: add the `"Mature Cheese"` → `MatureCheese` case, plus `"mature cheese"`, `"Mature Cheese Deluxe"` and `"MatureCheese"` → `Standard`.
- `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs`: add a `Mature_WhenKindIsMatureCheese_FollowsExpectedRules` theory, and the consecutive-updates and determinism tests.
- `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs`: add the end-to-end cases, mixed-inventory, multiple-item, similar-name, negative-quality and no-mutation tests.
- No changes under `src/Core/OriginalCode`. No new packages. No README or CI changes.

## Risks
- **Shared switch in `ItemMaturingPolicy` and `ItemKind.FromName`.**
  - A wrong arm order or a wildcard could change other kinds.
  - Mitigation: add only new arms before the default arm, and keep the existing tests unchanged (SC-003).
- **Public surface.** `ItemKind` gains a new nested type. Any external exhaustive `switch` on it would fall to its default arm, and the repository has no such code.
- **Off-by-one on the sell-by boundary.** The case `sellIn = 0` must give +2 (BR-03). Tests cover sellIn 1, 0 and -1.
- **Clamping.** Quality must stay in 0–50. This is handled by the existing `Quality` clamp, and tests cover 0, 1, 49, 50 and negative input.
- **OriginalCode.** FR-013 is verified by checking that the diff touches nothing under `src/Core/OriginalCode`.
- No migrations, persistence or UI are involved.

## Test plan
Test names follow the repository's `Method_WhenX_Y` style. The issue's suggested names serve as guidance for what each test covers.

| Requirement | Test |
|---|---|
| FR-001 | `ItemKindTests`: `"Mature Cheese"` maps to `ItemKind.MatureCheese`. A service-level test confirms the Mature Cheese rule is applied (AC-009). |
| FR-002 | `ItemMaturingPolicyTests`, Mature Cheese theory: sellIn 5→4, 1→0, 0→-1, -1→-2. |
| FR-003 | Policy theory: (q10, s5)→11, (q10, s1)→11, (q0, s5)→1, (q1, s5)→2. |
| FR-004 | Policy theory: (q10, s0)→12, (q10, s-1)→12, (q0, s0)→2. |
| FR-005 | Boundary rows sellIn 1 (+1) and 0 (+2) in the policy theory and the service theory. |
| FR-006 | Policy theory: (q49, s5)→50, (q49, s0)→50, (q48, s0)→50. |
| FR-007 | Policy theory: (q50, s5)→50 and (q50, s0)→50. |
| FR-008 | Service test: Mature Cheese with quality -5 and with quality above 50 gives a result in 0–50. A loop over several updates asserts quality stays in range (SC-006). |
| FR-009 | `ItemKindTests` and a service test: `mature cheese`, `Mature Cheese Deluxe` and `MatureCheese` follow the standard rule (for example q10, s5 → q9, s4). |
| FR-010 | The existing theories stay unchanged. A new mixed-inventory test mixes Mature Cheese with Aged Brie, Backstage, Sulfuras, Conjured and a standard item, and compares the non-cheese results with expected values and with `ItemMaturationService` results. |
| FR-011 | Service test: the input `Item` is not modified by `Mature`, and creating the item and reading its fields leaves quality and sellIn unchanged. |
| FR-012 | Service test: two updates from the same start give identical results. |
| FR-013 | Review `git diff --stat` and confirm there are no paths under `src/Core/OriginalCode`. |
| FR-014 | All the tests above. Also covered: consecutive updates (for example q10, s1 over 3 days → q11, q13, q15) and several Mature Cheese items in one inventory. |

Command from the repository root:
```
dotnet restore && dotnet build --no-restore && dotnet test --no-build --verbosity normal
```
To run only the new tests, use `dotnet test --filter "FullyQualifiedName~MatureCheese"`.