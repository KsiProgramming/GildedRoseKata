# Implementation plan: Add Mature Cheese

## Technical context

The proposed implementation uses C# on .NET 8. Product classification and maturation rules live in `src/Core/ProposedCode`; immutable `ItemKind`, `Quality`, `SellIn`, and `ItemState` types encapsulate the domain behavior. Tests use xUnit and FluentAssertions. CI restores, builds, and tests the solution with the .NET CLI.

## Approach

Treat the specification’s examples as confirming the proposed defaults: match the exact name `Mature Cheese`, increase quality by 1 when the starting `sellIn` is positive and by 2 otherwise, cap quality at 50, and add no other age-based behavior. Reuse `Quality`’s existing 0–50 normalization, which also handles out-of-range legacy quality when the item state is created.

Add a dedicated `ItemKind.MatureCheese`, map only the exact product name to it, and handle it explicitly in `ItemMaturingPolicy`. Base the quality increase on the state’s starting `SellIn`, then decrement `SellIn` once. Leave the existing product cases and original update service unchanged. Add focused classification and policy tests, and service tests for matching, bounds, and preservation of the input item.

## Files

- `src/Core/ProposedCode/Domain/ItemKind.cs` — add the Mature Cheese product kind and exact-name mapping.
- `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` — implement its increase, cap via `Quality`, and daily sell-by decrement.
- `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` — verify exact-name classification and similar names remain standard.
- `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` — cover sell-by boundaries, repeated updates, quality bounds, and existing product regressions.
- `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` — verify end-to-end name handling, independent items, and that maturation does not mutate the input item.

## Risks

`ItemKind` is a public domain type, so adding a case requires updating the policy’s dispatch. `Quality` clamps out-of-range input when creating an `ItemState`; tests should verify that this existing policy keeps Mature Cheese within bounds. The legacy `UpdateQualityService` has separate rules; changing it would risk altering established behavior, so the feature should remain in the proposed maturation path. No migration or UI changes are needed.

## Test plan

Run `dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj`.

- **FR-001, FR-007:** classification tests verify exact `Mature Cheese` matching and that similar names remain standard.
- **FR-002–FR-004:** policy tests verify +1 for positive starting `sellIn`, +2 for zero or negative starting `sellIn`, and a one-day decrement on each update.
- **FR-005–FR-006:** policy and quality-boundary tests verify quality remains within 0–50, including out-of-range starting values.
- **FR-008:** existing product policy and service regression tests verify other product behavior is unchanged.
- **FR-009:** service tests verify the input item remains unchanged; no creation, display, or search behavior is added.
- **FR-010–FR-011:** tests cover multiple independent Mature Cheese items, mixed product kinds, and deterministic results.
- **FR-012:** the targeted automated suite covers the new rules and existing product regressions.
- **FR-013:** tests verify only the specified quality and sell-by behavior is applied.