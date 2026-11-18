# Feature specification: Introduce Cellar-Aged Mead as a new maturing product

Issue: KAN-5

UI work: no

## User scenarios

### Scenario 1: The innkeeper stocks a maturing premium product
The innkeeper adds "Cellar-Aged Mead" to the inventory. The daily update recognises it as its own product and makes it more valuable as it ages, so it can be sold at a premium.

Acceptance scenarios:
1. Given a Cellar-Aged Mead with quality 20 and sell-in 5, when the daily inventory update executes, then quality becomes 21 and sell-in becomes 4.
2. Given a Cellar-Aged Mead with quality 20 and sell-in 1, when the daily inventory update executes, then quality becomes 21 and sell-in becomes 0.

### Scenario 2: The mead matures faster once it has reached its sell-by date
The mead keeps gaining value after its sell-by date, and gains it faster. It never loses quality because it is expired.

Acceptance scenarios:
1. Given a Cellar-Aged Mead with quality 20 and sell-in 0, when the daily inventory update executes, then quality becomes 22 and sell-in becomes -1.
2. Given a Cellar-Aged Mead with quality 30 and sell-in -5, when the daily inventory update executes, then quality becomes 32 and sell-in becomes -6.

### Scenario 3: Quality never goes above 50
Acceptance scenarios:
1. Given a Cellar-Aged Mead with quality 49 and sell-in 5, when the daily inventory update executes, then quality becomes 50 and never exceeds 50.
2. Given a Cellar-Aged Mead with quality 50 and sell-in 0, when the daily inventory update executes, then quality remains 50 and sell-in becomes -1.
3. Given a Cellar-Aged Mead with quality 49 and sell-in 0, when the daily inventory update executes, then quality becomes 50 and not 51.

### Scenario 4: Existing products behave as before
Acceptance scenarios:
1. Given an inventory containing normal items, Aged Brie, Backstage passes, Sulfuras and Conjured items, when the daily inventory update executes, then every one of them follows the rules it followed before this change.
2. Given an inventory with several Cellar-Aged Mead entries with different sell-in and quality values plus other products, when the daily inventory update executes, then each entry is updated only from its own values and no other item is affected.

## Requirements

- FR-001: The system shall recognise "Cellar-Aged Mead" as a supported product, identified by its explicit product name and not by a broad substring match. [NEEDS CLARIFICATION: Should the name match be exact and case-sensitive, like the existing product names? The code matches the other names exactly.]
- FR-002: The system shall increase the quality of Cellar-Aged Mead by 1 for each daily inventory update while the product is within its sell-by period (sell-in greater than 0).
- FR-003: The system shall increase the quality of Cellar-Aged Mead by 2 for each daily inventory update when the product has reached or passed its sell-by date (sell-in 0 or lower).
- FR-004: The system shall never let the quality of Cellar-Aged Mead exceed 50, including when an increase would take it past 50, and shall keep quality at 50 when it is already 50.
- FR-005: The system shall decrease the sell-in of Cellar-Aged Mead by exactly 1 on every daily inventory update, including when it is already negative or when quality is already 50.
- FR-006: The system shall not reduce the quality of an expired Cellar-Aged Mead because its sell-by date has passed.
- FR-007: The system shall preserve the existing rules for normal products, Aged Brie, Backstage passes, Sulfuras, Conjured and all other existing product types.
- FR-008: The system shall process each inventory entry independently, so that several Cellar-Aged Mead entries can coexist, each with its own sell-in and quality, and updating one affects no other item.
- FR-009: The system shall give the same result every time the same Cellar-Aged Mead values are updated, and repeated daily updates shall apply the rules cumulatively and deterministically.
- FR-010: The system shall keep the minimum quality of Cellar-Aged Mead at 0 and shall accept any configured initial quality within 0 to 50. [NEEDS CLARIFICATION: Must the legacy `OriginalCode/UpdateQualityService` also handle Cellar-Aged Mead, or only the `ProposedCode` maturation service (`ItemMaturationService`)? The repository has both update paths.]

## Success criteria

- SC-001: All seven acceptance scenarios from the issue (AC-01 to AC-07) pass as automated tests.
- SC-002: A Cellar-Aged Mead starting at quality 20 and sell-in 2, updated three days in a row, ends at quality 24 and sell-in -1 (+1, +1, +2).
- SC-003: A Cellar-Aged Mead never shows quality above 50 or below 0 after any number of daily updates.
- SC-004: The existing test suite for current product types passes with no change to its expected values.
- SC-005: Boundary values (sell-in 1, 0, -1; quality 0, 48, 49, 50) give the results the rules predict.
- SC-006: An inventory containing several Cellar-Aged Mead entries and other products gives each item the result it would get if updated alone.

## Acceptance criteria

- AC-001: When the daily update runs on a Cellar-Aged Mead with quality 20 and sell-in 5, the system shall set quality to 21 and sell-in to 4 (checks FR-002, FR-005).
- AC-002: When the daily update runs on a Cellar-Aged Mead with quality 20 and sell-in 0, the system shall set quality to 22 and sell-in to -1 (checks FR-003).
- AC-003: When the daily update runs on a Cellar-Aged Mead with quality 30 and sell-in -5, the system shall set quality to 32 and sell-in to -6 (checks FR-003, FR-006).
- AC-004: When the daily update runs on a Cellar-Aged Mead with quality 49 and sell-in 5, the system shall set quality to 50 (checks FR-004).
- AC-005: When the daily update runs on a Cellar-Aged Mead with quality 50 and sell-in 0, the system shall keep quality at 50 and set sell-in to -1 (checks FR-004, FR-005).
- AC-006: When the daily update runs on a Cellar-Aged Mead with quality 20 and sell-in 1, the system shall set quality to 21 and sell-in to 0 (checks FR-002, FR-005).
- AC-007: When the daily update runs on an inventory of existing product types, the system shall give each item the same quality and sell-in it gave before this change (checks FR-007).
- AC-008: When the daily update runs on a Cellar-Aged Mead with quality 49 and sell-in 0, the system shall set quality to 50 (checks FR-004).
- AC-009: When the daily update runs on several Cellar-Aged Mead entries with different sell-in values alongside other products, the system shall update each entry according to its own sell-in and leave every other item unaffected (checks FR-008).
- AC-010: When the daily update runs repeatedly on a Cellar-Aged Mead, the system shall apply +1 per day while sell-in is above 0, +2 per day once it is 0 or lower, cap at 50, and give the same results on every run with the same starting values (checks FR-009).
- AC-011: When the daily update runs on a Cellar-Aged Mead with quality 0 or 48, the system shall give 1 or 49 with sell-in above 0, and 2 or 50 with sell-in 0 or lower (checks FR-010, FR-002, FR-003).
- AC-012: When an item is named "Cellar-Aged Mead", the system shall treat it as a maturing product, and when an item's name only contains part of that name, the system shall not treat it as Cellar-Aged Mead (checks FR-001).
- AC-013: When the daily update runs on a Cellar-Aged Mead with a negative sell-in, the system shall not decrease its quality (checks FR-006).

## Out of scope

- Changing the rules for normal products, Aged Brie, Backstage passes, Sulfuras or Conjured items.
- Changing the `Item` class.
- Any user interface, pricing, packaging or ordering features (for example the "bundle of 6 bottles" marketing text in the issue's product definition).
- Fuzzy or partial name matching for Cellar-Aged Mead.
- Adding other new product types.

## Open questions

- [NEEDS CLARIFICATION: Should the name match be exact and case-sensitive, like the existing product names? The code matches the other names exactly.]
- [NEEDS CLARIFICATION: Must the legacy `OriginalCode/UpdateQualityService` also handle Cellar-Aged Mead, or only the `ProposedCode` maturation service (`ItemMaturationService`)? The repository has both update paths.]