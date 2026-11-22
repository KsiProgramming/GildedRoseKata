# Feature specification: Add Mature Cheese to Gilded Rose
gilded_rose_new_product_business_rules.md
UI work: no

## User scenarios

**Daily inventory update:** As an inventory manager, I want Mature Cheese quality and sell-by values updated each day so that inventory reflects its aging.

- Given Mature Cheese with `sellIn = 5` and `quality = 10`, when the inventory is updated, then it has `sellIn = 4` and `quality = 11`.
- Given Mature Cheese with `sellIn = 1` and `quality = 10`, when the inventory is updated, then it has `sellIn = 0` and `quality = 11`.
- Given Mature Cheese with `sellIn = 0` and `quality = 10`, when the inventory is updated, then it has `sellIn = -1` and `quality = 12`.
- Given Mature Cheese with `sellIn = -1` and `quality = 10`, when the inventory is updated, then it has `sellIn = -2` and `quality = 12`.

**Quality validation:** As an inventory manager, I want Mature Cheese quality kept within valid bounds so that invalid values are not produced.

- Given Mature Cheese at or near the quality limits, when the inventory is updated, then its quality remains between 0 and 50 inclusive.

**Mixed inventory:** As an inventory manager, I want Mature Cheese rules applied only to the matching product so that other inventory items retain their behavior.

- Given multiple Mature Cheese items and other products in one inventory, when the inventory is updated, then each Mature Cheese follows its rules and other products retain their existing rules.

**Read-only access:** As a user viewing or searching inventory, I want Mature Cheese values to remain unchanged until the daily update so that access does not alter inventory.

- Given a Mature Cheese item, when it is created, read, displayed, or searched, then its `quality` and `sellIn` remain unchanged.

## Requirements

FR-001. The system shall recognize `Mature Cheese` as an aging food product and apply its new behavior only to the exact product name. [NEEDS CLARIFICATION: Is the official product name `Mature Cheese`, or should it be another exact name?]

FR-002. When `sellIn` is greater than 0 before an update, the system shall increase Mature Cheese quality by 1. [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]

FR-003. When `sellIn` is 0 or less before an update, the system shall increase Mature Cheese quality by 2. [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]

FR-004. The system shall decrease Mature Cheese `sellIn` by 1 for each daily inventory update and update the item with the resulting values.

FR-005. The system shall cap Mature Cheese quality at 50 after an increase. [NEEDS CLARIFICATION: Is 50 the correct maximum quality?]

FR-006. The system shall keep Mature Cheese quality between 0 and 50 inclusive, including when input data has out-of-range quality; invalid imported values shall follow the existing quality-normalization policy.

FR-007. The system shall leave products whose names are similar but not an exact match to `Mature Cheese` under their existing rules.

FR-008. The system shall leave existing product behavior unchanged, including Aged Brie, Backstage passes, Sulfuras, and Conjured items.

FR-009. The system shall not change Mature Cheese `quality` or `sellIn` during creation, reading, displaying, or searching; values change only during the daily inventory update.

FR-010. The system shall apply the Mature Cheese rules independently to each matching item in inventories containing multiple Mature Cheese items or mixed product types.

FR-011. The system shall produce the same result for the same starting item and daily update operation.

FR-012. The system shall have automated unit-test coverage for the Mature Cheese business rules and regression coverage for existing product behavior.

FR-013. The system shall apply no additional Mature Cheese behavior, such as becoming unsafe or losing value after a maximum age, beyond the rules specified here. [NEEDS CLARIFICATION: Should Mature Cheese have any special behavior such as becoming unsafe or losing value after a maximum age?]

FR-014. The system shall confine implementation changes for this feature to proposed code and leave original code unchanged.

## Success criteria

SC-001. A daily update produces the specified results for starting `sellIn` values above zero, equal to zero, and below zero.

SC-002. Quality values of 0, 1, 49, and 50, including out-of-range imported values, result in Mature Cheese quality between 0 and 50 inclusive after an update.

SC-003. Multiple consecutive updates, multiple Mature Cheese items, mixed inventories, and similar-but-not-exact product names produce the specified behavior.

SC-004. Regression checks confirm that existing products retain their previous behavior and outputs.

SC-005. Automated tests cover the proposed cases, including `increases_quality_by_one_before_sell_by`, `increases_quality_by_two_on_sell_by_date`, `increases_quality_by_two_after_sell_by`, `does_not_increase_quality_above_fifty`, `keeps_quality_at_fifty`, `decreases_sell_in_by_one_each_day`, and `does_not_change_existing_product_behaviour`.

SC-006. Business rules are confirmed before implementation, and acceptance criteria pass as automated tests.

SC-007. Changes for this feature are confined to proposed code, and original code remains unchanged.

## Acceptance criteria

AC-001 (FR-001): When an item named exactly `Mature Cheese` is processed, the system shall apply the Mature Cheese aging-product rules. [NEEDS CLARIFICATION: Is the official product name `Mature Cheese`, or should it be another exact name?]

AC-002 (FR-002): When Mature Cheese has `sellIn = 5` or `sellIn = 1` and `quality = 10` at the start of an update, the system shall increase quality to 11. [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]

AC-003 (FR-003): When Mature Cheese has `sellIn = 0` or `sellIn = -1` and `quality = 10` at the start of an update, the system shall increase quality to 12. [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]

AC-004 (FR-004): When a daily update is performed on Mature Cheese, the system shall decrease its `sellIn` by 1 and return or store the updated values.

AC-005 (FR-005): When Mature Cheese with `sellIn = 5` and `quality = 49`, `sellIn = 0` and `quality = 49`, or quality 50 is updated, the system shall produce quality 50 in each case. [NEEDS CLARIFICATION: Is 50 the correct maximum quality?]

AC-006 (FR-006): When Mature Cheese with quality below 0 or above 50 is updated, the system shall apply the existing quality-normalization policy and produce a quality between 0 and 50 inclusive.

AC-007 (FR-007): When an item with a name similar to but not exactly `Mature Cheese` is updated, the system shall apply that item’s existing product rules.

AC-008 (FR-008): When Aged Brie, Backstage passes, Sulfuras, or Conjured items are updated, the system shall produce the same behavior and outputs as before this change.

AC-009 (FR-009): When Mature Cheese is created, read, displayed, or searched without a daily inventory update, the system shall leave its `quality` and `sellIn` unchanged.

AC-010 (FR-010): When an inventory containing multiple Mature Cheese items and other products is updated, the system shall apply the Mature Cheese rules to each matching item and existing rules to the other products.

AC-011 (FR-011): When the same starting item is subjected to the same daily update operation, the system shall produce the same result.

AC-012 (FR-012): When the Mature Cheese business rules or existing product behavior are changed, the system shall have automated unit and regression tests that verify the applicable outcomes.

AC-013 (FR-013): When Mature Cheese ages under the specified rules, the system shall not introduce additional age-based safety or value behavior. [NEEDS CLARIFICATION: Should Mature Cheese have any special behavior such as becoming unsafe or losing value after a maximum age?]

AC-014 (FR-014): When Mature Cheese support is added, the system shall confine implementation changes to proposed code and leave original code unchanged.

## Out of scope

- User-interface work.
- Changes to existing product behavior.
- Refactoring unrelated product behavior.
- Modifications to original code.
- Mature Cheese rules beyond the specified quality and sell-by behavior, pending clarification of any additional maximum-age behavior.

## Open questions

- [NEEDS CLARIFICATION: Is the official product name `Mature Cheese`, or should it be another exact name?]
- [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]
- [NEEDS CLARIFICATION: Is 50 the correct maximum quality?]
- [NEEDS CLARIFICATION: Should Mature Cheese have any special behavior such as becoming unsafe or losing value after a maximum age?]