# Feature specification: Add a New Product to Gilded Rose (Mature Cheese)

Issue: KAN-8

UI work: no

## User scenarios

The change applies only to the proposed code (`src/Core/ProposedCode`). The original code (`src/Core/OriginalCode`) stays as it is. The repository has no user interface. The update runs through `ItemMaturationService.Mature`, which today sorts items by name into Standard, Aged Brie, Backstage, Conjured and Sulfuras, and clamps quality to 0–50 (Sulfuras stays at 80).

### Scenario 1: Cheese improves before its sell-by date
A shopkeeper stocks Mature Cheese and runs the daily update. They want its quality to rise as it ages.
- Given a `Mature Cheese` item with sellIn 5 and quality 10, when the daily update runs, then sellIn is 4 and quality is 11.
- Given a `Mature Cheese` item with sellIn 1 and quality 10, when the daily update runs, then sellIn is 0 and quality is 11.

### Scenario 2: Cheese improves faster once the sell-by date is reached or passed
- Given a `Mature Cheese` item with sellIn 0 and quality 10, when the daily update runs, then sellIn is -1 and quality is 12.
- Given a `Mature Cheese` item with sellIn -1 and quality 10, when the daily update runs, then sellIn is -2 and quality is 12.

### Scenario 3: Quality never goes above 50
- Given a `Mature Cheese` item with sellIn 5 and quality 49, when the daily update runs, then quality is 50.
- Given a `Mature Cheese` item with sellIn 0 and quality 49, when the daily update runs, then sellIn is -1 and quality is 50.
- Given a `Mature Cheese` item with quality 50, when the daily update runs, then quality stays 50.

### Scenario 4: Quality never goes below 0
- Given a `Mature Cheese` item loaded with a negative quality, when the daily update runs, then the resulting quality is between 0 and 50.

### Scenario 5: Other products behave as before
- Given an inventory with Mature Cheese, Aged Brie, Backstage passes, Sulfuras, Conjured and standard items, when the daily update runs, then each non-Mature-Cheese item ends with the same sellIn and quality it had before this change.
- Given an item whose name is similar to but not exactly `Mature Cheese` (for example `mature cheese` or `Mature Cheese Deluxe`), when the daily update runs, then it follows the standard-item rule.

### Scenario 6: Only the update changes values
- Given a `Mature Cheese` item, when it is created, read or displayed, then its quality and sellIn do not change.
- Given several Mature Cheese items in one inventory, when the daily update runs several days in a row, then each item follows the rules independently and the same start always gives the same result.

## Requirements

- FR-001: The system shall recognise the exact product name `Mature Cheese` as a new product type with its own maturing rule.
- FR-002: The system shall decrease the sellIn of a Mature Cheese item by 1 at each update.
- FR-003: When sellIn is greater than 0 at the start of the update, the system shall increase Mature Cheese quality by 1.
- FR-004: When sellIn is 0 or less at the start of the update, the system shall increase Mature Cheese quality by 2.
- FR-005: The system shall decide the quality increase from the sellIn value before it is decremented.
- FR-006: The system shall never store a Mature Cheese quality above 50. If an increase would take it above 50, it shall store 50.
- FR-007: A Mature Cheese item already at quality 50 shall stay at quality 50.
- FR-008: The system shall never leave a Mature Cheese item with quality below 0, even if the item was loaded with invalid legacy data. Invalid quality shall be handled by the application's existing policy, which brings it into the range 0–50 before the rule is applied.
- FR-009: The new behaviour shall apply only to the exact name `Mature Cheese`. Other names shall follow their existing rules.
- FR-010: Existing products (Aged Brie, Backstage passes, Sulfuras, Conjured and standard items) shall keep their current behaviour and outputs.
- FR-011: The system shall not change the quality or sellIn of a Mature Cheese item except during the update operation.
- FR-012: The same starting Mature Cheese item shall always give the same result after an update.
- FR-013: The change shall be made only in the proposed code. The original code shall not be modified.
- FR-014: The Mature Cheese rule shall be isolated from the other product rules and covered by automated unit tests. These shall include the acceptance criteria, edge cases (quality 0, 1, 49 and 50; sellIn above, at and below 0), consecutive updates, multiple items, mixed inventories and similar names.

## Success criteria

- SC-001: All six example scenarios of the issue produce the stated ending sellIn and quality in automated tests.
- SC-002: Automated tests cover quality 0, 1, 49 and 50 and sellIn above, at and below 0, and pass.
- SC-003: Existing tests for all other products pass unchanged.
- SC-004: A mixed inventory test shows non-Mature-Cheese items give the same results as before.
- SC-005: A diff of the change shows no edits under `src/Core/OriginalCode`.
- SC-006: Across any sequence of updates, a Mature Cheese quality is never outside 0–50.

## Acceptance criteria

- AC-001: When a `Mature Cheese` item with sellIn 5 and quality 10 is updated once, the system shall give sellIn 4 and quality 11. (FR-002, FR-003)
- AC-002: When a `Mature Cheese` item with sellIn 1 and quality 10 is updated once, the system shall give sellIn 0 and quality 11. (FR-003, FR-005)
- AC-003: When a `Mature Cheese` item with sellIn 0 and quality 10 is updated once, the system shall give sellIn -1 and quality 12. (FR-004, FR-005)
- AC-004: When a `Mature Cheese` item with sellIn -1 and quality 10 is updated once, the system shall give sellIn -2 and quality 12. (FR-004)
- AC-005: When a `Mature Cheese` item with sellIn 5 and quality 49 is updated, the system shall give quality 50. (FR-006)
- AC-006: When a `Mature Cheese` item with sellIn 0 and quality 49 is updated, the system shall give quality 50 and sellIn -1. (FR-006)
- AC-007: When a `Mature Cheese` item at quality 50 is updated, the system shall keep quality at 50. (FR-007)
- AC-008: When a `Mature Cheese` item with a negative quality is updated, the system shall give a quality between 0 and 50. (FR-008)
- AC-009: When an item named exactly `Mature Cheese` is updated, the system shall apply the Mature Cheese rule. (FR-001)
- AC-010: When an item with a similar but not exact name such as `mature cheese` or `Mature Cheese Deluxe` is updated, the system shall apply the standard-item rule. (FR-009)
- AC-011: When Aged Brie, Backstage passes, Sulfuras, Conjured and standard items are updated, the system shall give the same sellIn and quality as before this change. (FR-010)
- AC-012: When a Mature Cheese item is created or read, the system shall leave its quality and sellIn unchanged. (FR-011)
- AC-013: When the same Mature Cheese item is updated twice from the same start, the system shall give identical results. (FR-012)
- AC-014: When the change is reviewed, the system shall show no modification to the files under `src/Core/OriginalCode`. (FR-013)
- AC-015: When the unit tests are run, the system shall include passing tests for the Mature Cheese rule, the edge cases and the mixed inventory. (FR-014)

## Out of scope

- Any change to `src/Core/OriginalCode`.
- Changes to the rules of Aged Brie, Backstage passes, Sulfuras, Conjured or standard items.
- Special Mature Cheese behaviour such as becoming unsafe or losing value after a maximum age.
- Rejecting invalid imported data. The existing policy of bringing quality into 0–50 is kept.
- Name aliases, case-insensitive matching or other spellings of `Mature Cheese`.
- Any user interface, persistence or import feature.