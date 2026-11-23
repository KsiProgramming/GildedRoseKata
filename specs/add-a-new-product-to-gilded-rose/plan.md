# Implementation plan: Add Mature Cheese to Gilded Rose

## Technical context

The proposed implementation is C# targeting .NET 8, built with the .NET SDK. The feature belongs to `src/Core/ProposedCode`, which classifies items with `ItemKind`, applies daily rules through `ItemMaturingPolicy`, and represents bounded quality with `Quality`. Tests use xUnit and FluentAssertions; CI runs `dotnet restore`, `dotnet build`, and `dotnet test`.

## Approach

Add a `MatureCheese` item kind for the exact name `Mature Cheese`, leaving similar names classified as standard. Add an isolated maturation rule that determines the quality increase from the item’s `sellIn` **before** decrementing it, then advances `sellIn` by one day. Reuse `Quality`’s existing clamping and increase behavior to preserve the 0–50 bounds and the established handling of imported out-of-range values.

Extend the proposed-code unit tests for name matching, daily policy boundaries, quality limits, repeated updates, multiple items, and mixed inventories. Keep `OriginalCode` unchanged. The existing model has no separate create/read/display/search behavior; confirm the maturation service does not mutate its input item, and do not add unrelated APIs.

## Files

- `src/Core/ProposedCode/Domain/ItemKind.cs` — recognize the exact Mature Cheese name.
- `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` — implement the isolated daily rule and dispatch for the new kind.
- `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` — cover exact and similar-name classification.
- `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` — cover sell-by boundaries, quality bounds, and preservation of existing product rules.
- `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` — cover end-to-end updates, repeated updates, mixed and multiple items, and input immutability.

## Risks

- The spec marks the exact product name, quality increases, and maximum quality as needing confirmation; this plan uses the proposed values: exact name `Mature Cheese`, +1 before sell-by, +2 on and after sell-by, and a maximum of 50.
- `Quality` already clamps values on construction and increase, so imported invalid values should follow that existing policy without changing shared quality logic.
- `ItemKind` and `ItemMaturingPolicy` are shared by existing proposed products; narrowly scoped dispatch and regression tests reduce the risk of changing their behavior.
- `OriginalCode` is explicitly out of scope and must remain unchanged.

## Test plan

Run `dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj`.

- **FR-001–FR-003:** Policy tests for `sellIn` values 5, 1, 0, and -1 verify daily decrement and the correct increase based on the pre-update value.
- **FR-004–FR-005:** Policy tests for qualities 0, 1, 49, 50, and out-of-range inputs verify results remain within 0–50.
- **FR-006:** Classification and service tests verify exact-name matching and that similar names retain standard behavior.
- **FR-007:** Regression tests for Aged Brie, Backstage passes, Sulfuras, Conjured, and standard items verify existing behavior; confirm no `OriginalCode` files change.
- **FR-008:** Service test verifies an update does not mutate the input item; no separate read, display, or search APIs are present in this code.
- **FR-009:** Service tests cover multiple Mature Cheese items, consecutive updates, and mixed inventories.
- **FR-010:** Deterministic expected-result tests for the same starting state verify repeatable behavior.