# Implementation plan: Add Bordeaux Wine as a new maturing product (KAN-4)

## Technical context

- **Language and runtime:** C# on .NET 9 (CI uses `dotnet-version: 9.0.x`). The solution is `GildedRoseKata.sln`, with `src/Core` for the code and `tests/Core.Tests` for the tests.
- **Test tools:** xUnit (`[Theory]`/`[InlineData]`/`[Fact]`) with FluentAssertions.
- **Build and test:** `dotnet restore`, `dotnet build --no-restore` and `dotnet test --no-build`. `.github/workflows/build.yml` runs these through `dotnet.yml`.
- **Style:** each file has a copyright header, and the project uses file-scoped namespaces. Namespaces are `GildedRoseKata.ProposedCode` and `GildedRoseKata.ProposedCode.Tests`. `.editorconfig` and `Directory.Build.props` apply, so follow the existing formatting and analyzer rules.
- **How the new service works today:**
  - `ItemMaturationService.Mature(Item)` calls `ItemKind.FromName(name)`.
  - It builds an `ItemState` with `Quality` (clamped to 0–50) and `SellIn`.
  - It delegates to `IMaturingPolicy.Mature(kind, state)`, implemented by `ItemMaturingPolicy`.
  - `ItemKind` is an abstract record with sealed nested records, one per kind.
  - Any name not in `FromName` falls through to `Standard`.
  - The service itself needs no change, because it only resolves a kind and delegates to the policy.

## Approach

Bordeaux Wine follows the same rule as Aged Brie, but it is a separate kind. That way it is recognised by its exact name and does not change Brie's behaviour (FR-008).

1. Add a sealed record `ItemKind.BordeauxWine : ItemKind`.
2. In `ItemKind.FromName`, add one exact-match case `"Bordeaux Wine" => new BordeauxWine()`. Do not add aliases such as "BordeauxWine"; the spec puts aliases out of scope. Matching is case-sensitive, as for the other names.
3. In `ItemMaturingPolicy.Mature`, add the case `ItemKind.BordeauxWine => this.MatureBordeauxWine(state)`.
4. Add a private `MatureBordeauxWine(ItemState)`, following the existing `MatureAgedBrie` pattern:
   - `newQuality = state.Quality.Increase()`.
   - `newSellIn = state.SellIn.NextDay()`.
   - If `newSellIn.IsExpired`, apply `newQuality.Increase()` again.
   - Return `new(newQuality, newSellIn)`.
   - `Quality` already clamps to 0–50 through `Math.Clamp`, which covers FR-004 and FR-005. It also covers the FR-006 sell-in decrement, since `NextDay()` does that.

   Check that `SellIn.IsExpired` means "value < 0", so that sell-in 1→0 gives +1 and sell-in 0→-1 gives +2. The Brie test cases `(20, 0, 22, -1)` show the same semantics.
5. Leave `Quality.FromItemKind` unchanged. Only Sulfuras is special-cased there, so Bordeaux Wine takes the normal clamped path.
6. Do not touch `OriginalCode/UpdateQualityService.cs` or `Item.cs` (FR-010).

I considered reusing `MatureAgedBrie` for both kinds. I prefer a separate method so the two products can diverge later (for example, when KAN-1 adds Cellar-Aged Mead). The rule is small enough that a little duplication is acceptable.

## Files

| File | Change | Why |
|---|---|---|
| `src/Core/ProposedCode/Domain/ItemKind.cs` | Add the `BordeauxWine` record and the `"Bordeaux Wine"` case in `FromName` | FR-001, FR-007 |
| `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` | Add the switch case and `MatureBordeauxWine` | FR-002 to FR-006, FR-009 |
| `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` | Add `[InlineData("Bordeaux Wine", typeof(ItemKind.BordeauxWine))]`. Also add a test that near-variants (`"bordeaux wine"`, `"BordeauxWine"`, `" Bordeaux Wine"`) resolve to `Standard`. | FR-001, FR-007 and the no-aliases out-of-scope rule |
| `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` | Add `Mature_WhenKindIsBordeauxWine_FollowsExpectedRules` | FR-002 to FR-006, FR-011 |
| `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` | Add Bordeaux Wine `InlineData` rows to the existing theory | End-to-end through the service (FR-009) |

No change to `ItemMaturationService.cs`, `IMaturingPolicy.cs`, `Quality.cs`, `SellIn.cs` or `ItemState.cs`. No change to `OriginalCode/`.

## Risks

- **Shared code:** `ItemKind.FromName` and `ItemMaturingPolicy.Mature` serve every product. The new switch arm must go before the `_` default arm. Adding a `FromName` arm cannot alter existing names, because the matches are exact strings.
- **Public interface:** adding a nested record to `ItemKind` is additive. If any code does an exhaustive match on kinds, the compiler or analyzers could warn. The search found only the policy's switch, which has a default arm.
- **Legacy code:** `OriginalCode/UpdateQualityService.cs` must remain byte-for-byte unchanged (SC-005). Verify with `git diff --stat -- src/Core/OriginalCode`. Bordeaux Wine in the legacy service will still behave as a normal item, which is intended.
- **Sell-in semantics:** the boundary relies on the `IsExpired` evaluation after `NextDay()`. The tests at sell-in 1 and 0 guard against an off-by-one error.
- **Migrations:** none.

## Test plan

Add these cases to the Bordeaux Wine policy theory as `(quality, sellIn, expectedQuality, expectedSellIn)`:

| FR | Case | Where |
|---|---|---|
| FR-001 | `ItemKind.FromName("Bordeaux Wine")` is `BordeauxWine` | `ItemKindTests` |
| FR-007 | Case and whitespace variants resolve to `Standard`; service-level wine quality never decreases (rows below) | `ItemKindTests` and service theory |
| FR-002 | `(20, 10, 21, 9)` and `(20, 1, 21, 0)` | `ItemMaturingPolicyTests` |
| FR-003 | `(20, 0, 22, -1)` and `(20, -5, 22, -6)` | `ItemMaturingPolicyTests` |
| FR-004 | `(50, 10, 50, 9)`, `(50, -1, 50, -2)`, `(49, 10, 50, 9)`, `(49, -1, 50, -2)` and `(49, 0, 50, -1)` | `ItemMaturingPolicyTests` |
| FR-005 | `(0, 10, 1, 9)` and `(0, -1, 2, -2)`. The result is never negative. | `ItemMaturingPolicyTests` |
| FR-006 | The `expectedSellIn` column in all rows above | `ItemMaturingPolicyTests` |
| FR-008 | The existing theories for Standard, Brie, Backstage, Conjured and Sulfuras pass unmodified (SC-004) | existing tests |
| FR-009 | Service rows: `("Bordeaux Wine", 20, 10, 21, 9)`, `("Bordeaux Wine", 20, 0, 22, -1)` and `("Bordeaux Wine", 50, 0, 50, -1)` | `ItemMaturationServiceTests` |
| FR-010 | `git diff --stat -- src/Core/OriginalCode/UpdateQualityService.cs` shows no output (SC-005) | manual or CI check |
| FR-011 | The boundary and limit rows above | all new tests |

Command to run everything:

```
dotnet restore && dotnet build --no-restore && dotnet test --no-build --verbosity normal
```

To run only the affected tests: `dotnet test --filter "FullyQualifiedName~ProposedCode"`.