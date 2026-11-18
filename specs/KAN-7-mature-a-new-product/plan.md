# Implementation plan: Mature Cheese, a new product with its own maturing rule (KAN-7)

## Technical context
- **Language and runtime:** C# (`LangVersion` latest, nullable enabled, implicit usings). Target framework is `net8.0` from `Directory.Build.props`. CI uses the .NET 9.0.x SDK.
- **Projects:** `src/Core/GildedRoseKata.Core.csproj` holds the code. `tests/Core.Tests/GildedRoseKata.Core.Tests.csproj` holds the tests.
- **Test tools:** xUnit 2.9.3 and FluentAssertions 8.10.0. The tests use `[Theory]` with `[InlineData]`.
- **Analyzers:** Roslynator and SonarAnalyzer run in Debug. `TreatWarningsAsErrors` is false.
- **CI:** `.github/workflows/build.yml` runs `dotnet restore`, `dotnet build --no-restore`, then `dotnet test --no-build`.
- **Code that handles maturing:**
  - `ItemKind` is an abstract record with one sealed nested record per kind. Its `FromName(string)` maps names to kinds.
  - `ItemMaturingPolicy.Mature(ItemKind, ItemState)` is a `switch` that picks one private method per kind.
  - `ItemMaturationService` ties these together.
  - `Quality` is clamped to 0–50 by its constructor. Only Sulfuras bypasses this, through `Quality.FromItemKind`.
  - `SellIn.NextDay()` and `SellIn.IsExpired` (value < 0) are used after the day has passed.
- `OriginalCode/Item.cs` and `OriginalCode/UpdateQualityService.cs` are out of scope and stay untouched.

## Approach
Add Mature Cheese as a new kind that follows the existing pattern. It needs a new record in `ItemKind`, a name mapping and a policy method.

1. **Kind (FR-001):** add `public sealed record MatureCheese : ItemKind;` to `ItemKind`.
2. **Recognition (FR-002):** add two cases to the `FromName` switch, `"Mature Cheese"` and `"MatureCheese"`. Both return `new MatureCheese()`. They go before the `_ => Standard` default. The match is exact and case-sensitive, like the other names. No other aliases are added.
3. **Rule (FR-003, FR-004, FR-005):**
   - Add the case `ItemKind.MatureCheese => this.MatureMatureCheese(state)` to the `ItemMaturingPolicy.Mature` switch.
   - Add a private method `MatureMatureCheese` that mirrors `MatureAgedBrie`:
     - `newQuality = state.Quality.Increase()`.
     - `newSellIn = state.SellIn.NextDay()`.
     - If `newSellIn.IsExpired`, apply `newQuality.Increase()` again.
     - Return `new(newQuality, newSellIn)`.
   - `IsExpired` on the decremented value is true exactly when the old SellIn is 0 or below. That gives +1 per day while SellIn is above 0 and +2 per day when it is 0 or below, which is what the spec asks for.
   - `Quality.Increase` clamps to 50. Quality 49 with expired SellIn gives 50, and Quality 50 stays 50. The quality never goes down.
   - The method is a separate, own-named rule and doesn't call the Aged Brie method, so the two kinds can change independently (FR-003).
4. **Name unchanged (FR-006):** `ItemMaturationService.Mature` already copies `item.Name`. No change is needed.
5. **Other kinds unchanged (FR-007):** the existing switch arms and methods are not edited. The new arm is added before the `_` default, and no existing name maps differently.
6. `Quality.FromItemKind`, `ItemState`, `SellIn` and `IMaturingPolicy` need no change. Mature Cheese is not a special case like Sulfuras, so it goes through the normal clamped `Quality`.

## Files
- `src/Core/ProposedCode/Domain/ItemKind.cs` (change): add the `MatureCheese` record and the two name mappings.
- `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` (change): add the switch arm and the private `MatureMatureCheese` method.
- `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` (change): add `InlineData` rows for `"Mature Cheese"` and `"MatureCheese"`. Both map to `ItemKind.MatureCheese`.
- `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` (change): add a theory `Mature_WhenKindIsMatureCheese_FollowsExpectedRules`.
- `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` (change): add rows for both names, plus a name-preserved check.

No other files are added. Existing test rows are not modified (SC-003).

## Risks
- **Shared switch and name mapping:** `FromName` and `Mature` are used by all kinds. The new arms come before the default, and the existing arms stay as they are. The existing tests would catch a regression.
- **Public interface:** the public surface only grows. `ItemKind` gets a new nested type, and `IMaturingPolicy` and the `Item` class don't change. Any outside code that switches over `ItemKind` exhaustively would fall into its default arm. The search found none in the repo besides `ItemMaturingPolicy` and `Quality.FromItemKind`, and both have defaults.
- **Boundary semantics:** the "SellIn 0 or below" rule relies on `IsExpired` being tested after `NextDay()`. The tests cover SellIn 1, 0 and -1 to guard this.
- **Naming:** the method name `MatureMatureCheese` reads awkwardly. It follows the existing `Mature<Kind>` convention, so keep it for consistency.
- **Migrations and data:** none.

## Test plan
Run `dotnet test` from the repo root. It restores and builds first. CI uses `dotnet restore`, `dotnet build --no-restore` and `dotnet test --no-build`.

| Requirement | Test | Cases |
|---|---|---|
| FR-001, FR-002 | `ItemKindTests.FromName_MapsNamesToExpectedKinds` | `"Mature Cheese"` and `"MatureCheese"` map to `ItemKind.MatureCheese` |
| FR-003 | `ItemMaturingPolicyTests.Mature_WhenKindIsMatureCheese_FollowsExpectedRules` (quality, sellIn → expected quality, expected sellIn) | (10,5→11,4), (10,1→11,0), (10,0→12,-1), (10,-3→12,-4) |
| FR-004 | Same theory, SellIn assertion | Every row asserts SellIn decreases by 1 |
| FR-005 | Same theory, limit rows | (49,5→50,4), (49,0→50,-1), (50,5→50,4), (50,0→50,-1), (0,5→1,4) |
| FR-003 (never decreases) | Same theory, assertions | Result quality ≥ input quality in all rows |
| FR-006 | `ItemMaturationServiceTests` | Both names come back unchanged; the result name equals the input name |
| FR-007, SC-003 | Existing tests in `ItemMaturingPolicyTests`, `ItemKindTests` and `ItemMaturationServiceTests` | All existing rows run unmodified and pass |
| SC-001 | `ItemMaturationServiceTests.Mature_UsesExpectedRulesForResolvedItemKinds` | `"Mature Cheese"` and `"MatureCheese"` with Quality 10 and SellIn 5 give 11 and 4. The existing `"Elixir of the Mongoose"` row gives 9 and 4. |
| SC-002 | The Mature Cheese policy theory | Covers SellIn above 0, SellIn 0, SellIn below 0 and the Quality limit of 50 |