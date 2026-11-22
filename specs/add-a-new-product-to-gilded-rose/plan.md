# Implementation plan: Add Mature Cheese to Gilded Rose

## Technical context

The proposed implementation is C# on .NET 8, with nullable reference types and the latest C# language version. Product rules use `ItemKind`, `ItemState`, and `Quality` value types, applied through `ItemMaturingPolicy` and `ItemMaturationService`. Tests use xUnit and FluentAssertions; CI runs `dotnet test`.

## Approach

Add a `MatureCheese` item kind and recognize only the exact name `Mature Cheese`. Route that kind to an isolated policy method that decides the quality increase from the starting `SellIn`, clamps quality using the existing `Quality` behavior, and decrements `SellIn` once. Preserve all existing policy branches and leave Original Code untouched.

Extend the proposed-code tests for exact-name recognition, before/on/after sell-by behavior, quality boundaries and invalid imported values, countdown, input immutability, mixed items, and regression behavior. Proceed with the values specified in the feature requirements: increases of 1 and 2, maximum quality 50, and no maximum-age penalty.

## Files

- `src/Core/ProposedCode/Domain/ItemKind.cs` — add the Mature Cheese kind and exact-name mapping.
- `src/Core/ProposedCode/Domain/ItemMaturingPolicy.cs` — add the isolated update rule without changing existing product rules.
- `tests/Core.Tests/ProposedCode/Domain/ItemKindTests.cs` — cover exact recognition and similar-but-not-exact names.
- `tests/Core.Tests/ProposedCode/Domain/ItemMaturingPolicyTests.cs` — cover quality increase based on pre-decrement sell-by value, decrement, and quality bounds.
- `tests/Core.Tests/ProposedCode/ItemMaturationServiceTests.cs` — cover service-level normalization, unchanged input, multiple items, and existing-product behavior.

## Risks

`ItemKind` and `ItemMaturingPolicy` are shared by existing products; changes there could affect their behavior, so retain existing mappings and branches and run the full proposed-code test suite. `Quality` already clamps to 0–50 for ordinary items, while Sulfuras has special handling; use that existing policy rather than changing the shared quality type. No public interface changes or migrations are expected. Original Code must remain unchanged.

## Test plan

Run `dotnet test tests/Core.Tests/GildedRoseKata.Core.Tests.csproj`.

- **FR-001, FR-007:** Item-kind tests verify exact `Mature Cheese` recognition and reject similar names.
- **FR-002–FR-005:** Policy tests verify decrement, +1 when starting `sellIn > 0`, +2 when starting `sellIn <= 0`, and the 50 ceiling.
- **FR-006:** Service tests verify qualities below 0 and above 50 are normalized and remain in range after update; boundary cases cover 0, 1, 49, and 50.
- **FR-008, FR-012, FR-013:** Existing product tests and mixed-item cases verify existing behavior remains unchanged; keep production edits confined to Proposed Code.
- **FR-009:** Service tests verify processing an item does not mutate the input item; no update operation means no value changes.
- **FR-010–FR-011:** Policy tests verify no added maximum-age behavior and deterministic results for repeated identical starting states.