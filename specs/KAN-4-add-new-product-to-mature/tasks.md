# Tasks: Add Bordeaux Wine as a new maturing product (KAN-4)

## Tests first
- [ ] T001 [P] Add `[InlineData("Bordeaux Wine", typeof(ItemKind.BordeauxWine))]` to the existing `FromName` theory. Add a new test that `"bordeaux wine"`, `"BordeauxWine"` and `" Bordeaux Wine"` resolve to `ItemKind.Standard`, in tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs (FR-001, FR-007)
- [ ] T002 [P] Add `Mature_WhenKindIsBordeauxWine_FollowsExpectedRules` in tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs (FR-002, FR-003, FR-004, FR-005, FR-006, FR-011).
  - Use the `(quality, sellIn, expectedQuality, expectedSellIn)` rows `(20, 10, 21, 9)`, `(20, 1, 21, 0)`, `(20, 0, 22, -1)` and `(20, -5, 22, -6)`.
  - Add the limit rows `(50, 10, 50, 9)`, `(50, -1, 50, -2)`, `(49, 10, 50, 9)`, `(49, -1, 50, -2)` and `(49, 0, 50, -1)`.
  - Add the lower-bound rows `(0, 10, 1, 9)` and `(0, -1, 2, -2)`.
- [ ] T003 [P] Add the rows `("Bordeaux Wine", 20, 10, 21, 9)`, `("Bordeaux Wine", 20, 0, 22, -1)` and `("Bordeaux Wine", 50, 0, 50, -1)` to the existing theory in tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs (FR-007, FR-009, FR-011)

## Implementation
- [ ] T004 [P] Add `public sealed record BordeauxWine : ItemKind;` and the exact-match arm `"Bordeaux Wine" => new BordeauxWine()` before the `_` default in `FromName`. Do not add aliases. Both changes go in src/Core/ProposedCode/Domain/ItemKind.cs (FR-001, FR-007)
- [ ] T005 Add the arm `ItemKind.BordeauxWine => this.MatureBordeauxWine(state)` before the `_` default arm. Add a private `MatureBordeauxWine(ItemState)` that follows `MatureAgedBrie`. It calls `Quality.Increase()` and `SellIn.NextDay()`, and it increases quality again when `newSellIn.IsExpired`. Both changes go in src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs, after T004 because it needs the `ItemKind.BordeauxWine` type (FR-002, FR-003, FR-004, FR-005, FR-006, FR-009)

## Verification
- [ ] T006 Run `dotnet restore && dotnet build --no-restore && dotnet test --no-build --verbosity normal`. Confirm that the new Bordeaux Wine tests pass and that the existing tests for the other products pass unmodified. The step changes no files; the files it checks are tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs, tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs and tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs. Run it after T001–T005 (FR-008, FR-011)
- [ ] T007 Run `git diff --stat -- src/Core/OriginalCode`. Confirm that it prints nothing, so src/Core/OriginalCode/UpdateQualityService.cs is unchanged. The step changes no files. Run it after T005 (FR-010)