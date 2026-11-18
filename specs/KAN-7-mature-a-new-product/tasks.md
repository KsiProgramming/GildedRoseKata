# Tasks: Mature Cheese, a new product with its own maturing rule (KAN-7)

- [ ] T001 [P] Add `InlineData` rows `"Mature Cheese"` and `"MatureCheese"`, both expecting `ItemKind.MatureCheese`, to `FromName_MapsNamesToExpectedKinds`. Leave the existing rows unchanged. Edit `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` (FR-001, FR-002, AC-001)
- [ ] T002 [P] Add the theory `Mature_WhenKindIsMatureCheese_FollowsExpectedRules` (quality, sellIn → expected quality, expected sellIn) in `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs`. Leave the existing tests unchanged. (FR-003, FR-004, FR-005, AC-002, AC-003, AC-004)
  - Rows: (10,5→11,4), (10,1→11,0), (10,0→12,-1), (10,-3→12,-4), (49,5→50,4), (49,0→50,-1), (50,5→50,4), (50,0→50,-1), (0,5→1,4).
  - Every row asserts that SellIn drops by 1 and that the resulting Quality is at least the input Quality.
- [ ] T003 [P] In `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs`, add `"Mature Cheese"` and `"MatureCheese"` rows (Quality 10, SellIn 5 → Quality 11, SellIn 4) to `Mature_UsesExpectedRulesForResolvedItemKinds`. Also add a test that the returned item keeps the input name for both names. Leave the existing rows unchanged. (FR-002, FR-006, SC-001, AC-001, AC-005)
- [ ] T004 [P] Add `public sealed record MatureCheese : ItemKind;` to `ItemKind`. Add the `"Mature Cheese"` and `"MatureCheese"` cases to the `FromName` switch, both returning `new MatureCheese()` and placed before the `_ => new Standard()` default. Leave the existing arms unchanged. Edit `src/Core/ProposedCode/Domain/ItemKind.cs` (FR-001, FR-002, FR-007, AC-001, AC-006)
- [ ] T005 Add the arm `ItemKind.MatureCheese => this.MatureMatureCheese(state)` before the default in `ItemMaturingPolicy.Mature`. Add the private method `MatureMatureCheese` after T004, in `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs`. (FR-003, FR-004, FR-005, FR-007, AC-002, AC-003, AC-004, AC-006)
  - The method calls `state.Quality.Increase()` and `state.SellIn.NextDay()`.
  - If the new SellIn `IsExpired`, it calls `Increase()` once more.
  - It returns `new(newQuality, newSellIn)`.
  - It does not call `MatureAgedBrie`, and it leaves the existing arms and methods unchanged.
- [ ] T006 Run `dotnet restore`, `dotnet build --no-restore` and `dotnet test --no-build` from the repo root, after T001–T005. Confirm that the new Mature Cheese tests pass and that all existing tests pass unmodified. No files change. (FR-007, SC-002, SC-003, AC-006)