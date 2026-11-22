# Feature specification: Add Mature Cheese to Gilded Rose
spec.md

UI work: no

## User scenarios

**Daily inventory update:** As the inventory update process, I want Mature Cheese quality and sell-by values updated each day so that its quality improves according to its age.

- Given a Mature Cheese item with `sellIn` above zero, when the inventory is updated, then quality increases by 1 and `sellIn` decreases by 1.
- Given a Mature Cheese item with `sellIn` equal to or below zero, when the inventory is updated, then quality increases by 2 and `sellIn` decreases by 1; the increase is based on `sellIn` before it is decremented.

**Protect quality values:** As the inventory system, I want Mature Cheese quality kept within valid bounds so that invalid quality values are not stored after an update.

- Given Mature Cheese at or near either quality boundary, when the inventory is updated, then its quality remains between 0 and 50 inclusive.

**Preserve other inventory behavior:** As the inventory manager, I want the new product rule limited to the exact product name and proposed code so that existing product behavior is preserved.

- Given a mixed inventory, when it is updated, then only the exact Mature Cheese product receives the new rule, and existing product behavior remains unchanged.

## Requirements

FR-001 The system shall recognize `Mature Cheese` as an aging food product with an initial quality in the range 0–50 and a sell-by value measured in days remaining. [NEEDS CLARIFICATION: Is the official product name `Mature Cheese`, or should it be another exact name?]

FR-002 At each daily inventory update, the system shall decrease Mature Cheese `sellIn` by 1.

FR-003 When Mature Cheese `sellIn` is greater than 0 at the start of an update, the system shall increase its quality by 1. [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]

FR-004 When Mature Cheese `sellIn` is 0 or less at the start of an update, the system shall increase its quality by 2. [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]

FR-005 The system shall cap Mature Cheese quality at 50 after an update. [NEEDS CLARIFICATION: Is 50 the correct maximum quality?]

FR-006 The system shall keep Mature Cheese quality between 0 and 50 inclusive after an update, including when imported quality is outside that range; invalid imported values shall follow the existing policy of normalizing to the range.

FR-007 The system shall apply the new behavior only to the exact product name `Mature Cheese`. [NEEDS CLARIFICATION: Is the official product name `Mature Cheese`, or should it be another exact name?]

FR-008 The system shall preserve existing behavior for other product types, including Aged Brie, Backstage passes, Sulfuras, and Conjured items.

FR-009 Creating, reading, displaying, or searching for a Mature Cheese item shall not change its quality or `sellIn`; these values shall change only during the daily inventory update.

FR-010 The system shall not add behavior that makes Mature Cheese unsafe or reduces its value after a maximum age unless that behavior is confirmed. [NEEDS CLARIFICATION: Should Mature Cheese have special behavior such as becoming unsafe or losing value after a maximum age?]

FR-011 The system shall produce the same result for the same starting item and daily update operation.

FR-012 The new rule shall be isolated and covered by automated unit tests, including the suggested cases `increases_quality_by_one_before_sell_by`, `increases_quality_by_two_on_sell_by_date`, `increases_quality_by_two_after_sell_by`, `does_not_increase_quality_above_fifty`, `keeps_quality_at_fifty`, `decreases_sell_in_by_one_each_day`, and `does_not_change_existing_product_behaviour`.

FR-013 The change shall be limited to Proposed Code and shall not change Original Code.

## Success criteria

SC-001 A Mature Cheese item with `sellIn = 5` and `quality = 10` updates to `sellIn = 4` and `quality = 11`.

SC-002 A Mature Cheese item with `sellIn = 1` and `quality = 10` updates to `sellIn = 0` and `quality = 11`; with `sellIn = 0` and `quality = 10`, it updates to `sellIn = -1` and `quality = 12`; with `sellIn = -1` and `quality = 10`, it updates to `sellIn = -2` and `quality = 12`.

SC-003 Updates keep quality within 0–50 for starting qualities 0, 1, 49, and 50, including multiple consecutive updates and invalid imported values.

SC-004 Multiple Mature Cheese items update correctly in the same inventory, and similar-but-not-exact product names and other product types retain their existing behavior.

SC-005 Existing product behavior and outputs remain unchanged, and Original Code remains unchanged.

## Acceptance criteria

AC-001 When a Mature Cheese item is created and updated, the system shall recognize the specified product and valid initial quality and treat `sellIn` as days remaining (FR-001).

AC-002 When a Mature Cheese item undergoes a daily update, the system shall decrease `sellIn` by exactly 1 (FR-002).

AC-003 When an update starts with Mature Cheese `sellIn` greater than 0, the system shall increase quality by exactly 1 (FR-003).

AC-004 When an update starts with Mature Cheese `sellIn` equal to or below 0, the system shall increase quality by exactly 2 using the pre-decrement `sellIn` value (FR-004).

AC-005 When a quality increase would put Mature Cheese above 50, the system shall store quality as 50 (FR-005).

AC-006 When an update processes Mature Cheese with quality below 0 or above 50, the system shall leave its updated quality within 0–50, applying the existing normalization policy to invalid imported values (FR-006).

AC-007 When an item name is exactly `Mature Cheese`, the system shall apply the new rule, and when its name differs, the system shall not apply that rule (FR-007).

AC-008 When an inventory containing existing product types is updated, the system shall preserve their previous behavior, including the named special products (FR-008).

AC-009 When a Mature Cheese item is created, read, displayed, or searched without a daily update, the system shall leave its quality and `sellIn` unchanged (FR-009).

AC-010 When Mature Cheese is updated at any age, the system shall apply no unconfirmed unsafe-product or maximum-age value-loss behavior (FR-010).

AC-011 When the same starting item is processed by the same daily update operation, the system shall produce the same updated values (FR-011).

AC-012 When the Mature Cheese behavior is covered by automated unit tests, the system shall include tests for the listed before-sell-by, on-sell-by, after-sell-by, quality-boundary, sell-by countdown, and existing-product behavior cases (FR-012).

AC-013 When the Mature Cheese change is made, the system shall leave Original Code unchanged and limit the implementation change to Proposed Code (FR-013).

## Out of scope

- User-interface changes.
- Changes to existing product rules or unrelated refactoring.
- Changes to Original Code.
- Additional Mature Cheese behavior beyond the proposed rules, pending clarification about maximum-age behavior.

## Open questions

- [NEEDS CLARIFICATION: Is the official product name `Mature Cheese`, or should it be another exact name?]
- [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]
- [NEEDS CLARIFICATION: Is 50 the correct maximum quality?]
- [NEEDS CLARIFICATION: Should Mature Cheese have special behavior such as becoming unsafe or losing value after a maximum age?]