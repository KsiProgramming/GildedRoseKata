# Tasks: Introduce a new maturing product (Vintage Wine)

- [ ] T001 Add `public sealed record VintageWine : ItemKind;` and the exact-name mappings `"Vintage Wine"` and `"Vintage"` to `VintageWine` in `FromName`. Place the mappings before the `_` default arm, and leave the existing arms alone. Do this in src/Core/ProposedCode/Domain/ItemKind.cs (FR-001, AC-001)
- [ ] T002 [P] Add `InlineData` rows to the `FromName` mapping theory so that "Vintage Wine" and "Vintage" give `ItemKind.VintageWine`. Keep the existing rows unchanged. Do this in tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs, after T001 because the rows need the `VintageWine` type to compile (FR-001, AC-001)
- [ ] T003 Add the arm `ItemKind.VintageWine => this.MatureVintageWine(state)` before the `_` default arm. Add a private `MatureVintageWine` method. It uses the start-of-day SellIn: `state.SellIn.IsBelow(11)` gives `state.Quality.Increase(2)`, and otherwise `state.Quality.Increase()`. SellIn becomes `state.SellIn.NextDay()`. It applies no expiry penalty and no reset. Do this in src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs, after T001 (FR-002, FR-003, FR-004, FR-005, FR-006, AC-002, AC-003, AC-004, AC-005, AC-006)
- [ ] T004 [P] Add a `Mature_WhenKindIsVintageWine_FollowsExpectedRules` theory with `InlineData` rows. It uses the quality-and-SellIn pairs below, and the expected SellIn is always the input minus 1.
  - Q10 with SellIn 20 and 11 gives Q11.
  - Q10 with SellIn 10, 1, 0 and -5 gives Q12.
  - Q50 with SellIn 20 gives Q50.
  - Q49 with SellIn 5 gives Q50.
  - Q49 with SellIn 20 gives Q50.
  - Do this in tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs, after T001 and T003. Leave the existing tests unchanged (FR-002 to FR-007, AC-002 to AC-007, SC-001, SC-002)
- [ ] T005 [P] Add Vintage Wine rows to `Mature_UsesExpectedRulesForResolvedItemKinds`. Each row is (name, quality, sellIn → expected quality, expected sellIn); use "Vintage" as the name in one row.
  - ("Vintage Wine", 10, 20 → 11, 19)
  - (10, 11 → 11, 10)
  - (10, 10 → 12, 9)
  - (10, 1 → 12, 0)
  - (10, 0 → 12, -1)
  - (10, -5 → 12, -6)
  - (50, 20 → 50, 19)
  - (49, 5 → 50, 4)
  
  Also add a `[Fact]` that asserts `result.Name == "Vintage Wine"` after `Mature`. Do this in tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs, after T001 and T003. Leave the existing rows unchanged (FR-007, FR-008, AC-007, AC-008, SC-002, SC-004)
- [ ] T006 Run `dotnet restore`, `dotnet build --no-restore` and `dotnet test --no-build` from the repository root, after T002, T004 and T005. Check that all tests pass, the existing Standard, Aged Brie, Backstage, Conjured and Sulfuras expectations are unchanged, and the build shows no new Roslynator or Sonar warnings. No file changes are expected (FR-007, SC-001, SC-002, SC-003)