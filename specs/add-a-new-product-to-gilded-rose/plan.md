# Implementation plan: Add Mature Cheese

## Technical context

The proposed implementation is C# on .NET 8, using xUnit and FluentAssertions. Product classification and maturation rules live in the Proposed Code domain; `Quality` already clamps values to 0–50. CI runs `dotnet test`.

## Approach

Add a `MatureCheese` item kind for the exact name `Mature Cheese`. Handle it in a dedicated policy method: choose a quality increase of 1 when the starting `sellIn` is positive, otherwise 2; let the existing `Quality` value object enforce the 0–50 range; then decrement `sellIn` once. Keep other policy branches unchanged. The service already returns updated item values without mutating the input item.

Use the defined name, increases, and cap as the implementation defaults. Add no behavior beyond the stated post-sell-by increase because no additional maximum-age rule is specified. Do not modify Original Code.

## Files

- `src/Core/ProposedCode/Domain/ItemKind.cs` — add exact-name classification for Mature Cheese.
- `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` — add the isolated Mature Cheese update rule.
- `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` — test exact classification and similar non-matching names.
- `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` — test before, on, and past sell-by behavior; sellIn countdown; quality boundaries and invalid imported values.
- `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` — test the returned updated values, multiple and consecutive updates in mixed inventories, unchanged input items, and existing product regressions.

## Risks

- The item kind and policy are shared dispatch points; an incorrect switch branch could alter existing product behavior. Preserve all existing branches and cover their current behavior with regression tests (FR-010).
- `Quality` clamps on state creation as well as when increased. Verify out-of-range values through the service path so tests reflect the application’s normalization policy (FR-008).
- The spec leaves additional maximum-age behavior undecided. Implement only the explicit rule for `sellIn <= 0`; do not add a separate age threshold (FR-014).
- Limit source changes to Proposed Code to preserve Original Code (FR-017).

## Test plan

Run `dotnet test` (the CI test command).

- `ItemKindTests`: verify exact-name classification and near-match exclusion (FR-001, FR-009).
- `ItemMaturingPolicyTests`: verify +1 for positive starting `sellIn`, +2 for zero or negative starting `sellIn`, decrement by one using the starting value, cap at 50, floor at 0, and repeated deterministic updates (FR-002–FR-008, FR-014–FR-016).
- `ItemMaturationServiceTests`: verify returned quality and `sellIn`, multiple matching items and consecutive updates in mixed inventories, and that the input item remains unchanged outside the update result (FR-011–FR-013, FR-015).
- Existing product cases in policy and service tests, plus the full test suite: verify established product behavior remains unchanged and Original Code is untouched (FR-010, FR-017–FR-019).