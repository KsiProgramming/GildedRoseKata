# Tasks: Add Mature Cheese product (KAN-8)

All changes go under `src/Core/ProposedCode` and `tests/Core.Tests`. Nothing under `src/Core/OriginalCode` is edited (FR-013).

- [ ] T001 [P] Add the `"Mature Cheese"` → `ItemKind.MatureCheese` case to `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs`. Add `"mature cheese"`, `"Mature Cheese Deluxe"` and `"MatureCheese"` → `ItemKind.Standard`. Use the existing `[Theory]`/`[InlineData]` style and keep the existing cases unchanged. (FR-001, FR-009, AC-009, AC-010)
- [ ] T002 [P] Add a `Mature_WhenKindIsMatureCheese_FollowsExpectedRules` theory to `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs`, with these rows (quality, sellIn → quality, sellIn):
  - (10, 5 → 11, 4)
  - (10, 1 → 11, 0)
  - (10, 0 → 12, -1)
  - (10, -1 → 12, -2)
  - (0, 5 → 1)
  - (1, 5 → 2)
  - (0, 0 → 2)
  - (48, 0 → 50)
  - (49, 5 → 50)
  - (49, 0 → 50, -1)
  - (50, 5 → 50)
  - (50, 0 → 50)

  Also add a consecutive-updates test (q10, s1 over 3 days gives q11, q13, q15) and a determinism test (two updates from the same start give identical results). Leave the existing tests unchanged. (FR-002, FR-003, FR-004, FR-005, FR-006, FR-007, FR-012, FR-014, AC-001, AC-002, AC-003, AC-004, AC-005, AC-006, AC-007, AC-013, AC-015)
- [ ] T003 [P] Add end-to-end `ItemMaturationService.Mature` tests to `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs`:
  - The acceptance rows for `Mature Cheese`.
  - Negative quality (-5) and quality above 50 give a result in 0–50. A multi-day loop keeps quality in 0–50 (SC-006).
  - Similar names (`mature cheese`, `Mature Cheese Deluxe`, `MatureCheese`) follow the standard rule (q10, s5 → q9, s4).
  - A mixed inventory with Mature Cheese, Aged Brie, Backstage, Sulfuras, Conjured and a standard item. The non-cheese items match their expected values and the current service output.
  - Several Mature Cheese items run over several days, each following the rules independently.
  - `Mature` does not modify the input `Item`, and creating and reading a Mature Cheese item leaves its quality and sellIn unchanged.

  Leave the existing tests unchanged. (FR-001, FR-005, FR-008, FR-009, FR-010, FR-011, FR-012, FR-014, AC-008, AC-009, AC-010, AC-011, AC-012, AC-013, AC-015)
- [ ] T004 [P] Add `public sealed record MatureCheese : ItemKind;` and the arm `"Mature Cheese" => new MatureCheese()` to `FromName` in `src/Core/ProposedCode/Domain/ItemKind.cs`. Put the new arm before the `_ => new Standard()` arm and leave the existing arms unchanged. (FR-001, FR-009, FR-010, FR-013, AC-009, AC-010)
- [ ] T005 Add the case `ItemKind.MatureCheese => this.MatureCheese(state)` and a private `MatureCheese(ItemState)` method in `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs`, after T004. The method:
  - reads the increase from the pre-decrement sellIn: `state.SellIn.Value > 0 ? 1 : 2`;
  - sets `newQuality = state.Quality.Increase(increase)`, which clamps to 50;
  - sets `newSellIn = state.SellIn.NextDay()`;
  - returns `new(newQuality, newSellIn)`.

  Do not edit the other kind methods, `Quality`, `SellIn`, `ItemState`, `IMaturingPolicy` or `ItemMaturationService`. (FR-002, FR-003, FR-004, FR-005, FR-006, FR-007, FR-008, FR-010, FR-011, FR-012, FR-013, FR-014, AC-001, AC-002, AC-003, AC-004, AC-005, AC-006, AC-007, AC-008)
- [ ] T006 Verify after T001–T005 by running `dotnet restore && dotnet build --no-restore && dotnet test --no-build --verbosity normal` from the repository root. All new and existing tests must pass. Then run `git diff --stat` and confirm it lists no path under `src/Core/OriginalCode`. No files change. (FR-010, FR-013, FR-014, SC-001, SC-002, SC-003, SC-004, SC-005, SC-006, AC-011, AC-014, AC-015)