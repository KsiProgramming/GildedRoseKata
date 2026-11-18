# Tasks: Introduce Cellar-Aged Mead as a new maturing product (KAN-5)

- [ ] T001 [P] Add a sealed record `CellarAgedMead : ItemKind` and the exact, case-sensitive arm `"Cellar-Aged Mead" => new CellarAgedMead()` in `ItemKind.FromName`, before the `_` default. Use no `Contains` or substring matching, and keep the copyright header and `this.` conventions. Change `src/Core/ProposedCode/Domain/ItemKind.cs` (FR-001, AC-012)
- [ ] T002 [P] Add the mead mapping tests after T001. Add `[InlineData("Cellar-Aged Mead", typeof(ItemKind.CellarAgedMead))]` to the existing theory. Add a `Standard` theory for "Cellar-Aged", "Mead", "Cellar-Aged Mead Reserve", "cellar-aged mead" and "Cellar-Aged Mead " (trailing space). Change `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` (FR-001, AC-012)
- [ ] T003 [P] Add the maturation rule after T001. Add the arm `ItemKind.CellarAgedMead => this.MatureCellarAgedMead(state)` before `_` in `ItemMaturingPolicy.Mature`. Add a private `MatureCellarAgedMead` that follows the `MatureAgedBrie` pattern:
  - `newQuality = state.Quality.Increase()`, `newSellIn = state.SellIn.NextDay()`.
  - If `newSellIn.IsExpired`, then `newQuality = newQuality.Increase()`.
  - Return `new(newQuality, newSellIn)`.
  - Do not reuse or edit `MatureAgedBrie`, and do not touch the other arms.

  Change `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` (FR-002, FR-003, FR-004, FR-005, FR-006, FR-007, FR-009, FR-010, AC-001, AC-002, AC-003, AC-004, AC-005, AC-006, AC-008, AC-013)
- [ ] T004 [P] Add the policy tests after T003. Add the theory `Mature_WhenKindIsCellarAgedMead_FollowsExpectedRules` with `(quality, sellIn, expectedQuality, expectedSellIn)` rows:
  - Rows: (20,5,21,4), (20,1,21,0), (20,0,22,-1), (30,-5,32,-6), (20,-1,22,-2), (49,5,50,4), (49,0,50,-1), (48,0,50,-1), (48,5,49,4), (50,5,50,4), (50,0,50,-1), (0,5,1,4), (0,0,2,-1).
  - Add a test for repeated updates: (20, 2) over 3 days gives (24, -1).
  - Add tests that updates from quality 0 and from quality 48 are capped at 50.
  - Add a determinism test: two runs from the same state give equal results.
  - Add a long-loop test that quality stays within 0–50.
  - Add a test that a negative sell-in never lowers quality.

  Change `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` (FR-002, FR-003, FR-004, FR-005, FR-006, FR-009, FR-010, AC-001, AC-002, AC-003, AC-004, AC-005, AC-006, AC-008, AC-010, AC-011, AC-013)
- [ ] T005 [P] Add the service-level and regression tests after T003.
  - Add mead rows to the existing `Mature_UsesExpectedRulesForResolvedItemKinds` theory: ("Cellar-Aged Mead", 20, 5, 21, 4) and ("Cellar-Aged Mead", 20, 0, 22, -1).
  - Add a mixed-inventory test with several mead entries at different sell-in and quality values plus Standard, Aged Brie, Backstage, Conjured and Sulfuras items. Assert each result equals the result of updating that item alone, and assert the input items are not mutated.
  - Add a regression test over all existing kinds that asserts the known pre-change outputs.
  - Add a test that a partial or variant name such as "Cellar-Aged Mead Reserve" is treated as a standard item that loses quality.

  Change `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` (FR-001, FR-007, FR-008, FR-009, AC-007, AC-009, AC-012)
- [ ] T006 [P] Add a short note under the kata description that "Cellar-Aged Mead" increases in quality by 1 per day, by 2 once sell-in is 0 or lower, up to a maximum of 50. Change `README.md` (FR-002, FR-003, FR-004)
- [ ] T007 Run `dotnet build` and `dotnet test` from the repo root after T002, T004 and T005. Confirm the new tests pass, the existing expected values are unchanged, and that `Item.cs` and `UpdateQualityService.cs` are not modified. This changes no files. (FR-007, AC-007)