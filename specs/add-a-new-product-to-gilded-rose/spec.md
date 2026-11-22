# Feature specification: Add a New Product to Gilded Rose
UI work: no
spec.md

## User scenarios

**Mature Cheese before its sell-by date**  
Given a Mature Cheese item with `sellIn` greater than 0, when the inventory is updated, then its quality increases by 1 and its `sellIn` decreases by 1.

**Mature Cheese at or past its sell-by date**  
Given a Mature Cheese item with `sellIn` equal to or less than 0 at the start of an update, when the inventory is updated, then its quality increases by 2 and its `sellIn` decreases by 1.

**Quality boundaries and legacy data**  
Given Mature Cheese quality at a boundary or outside the valid range, when the inventory is updated, then its quality remains within 0–50.

**Mixed inventory and ordinary access**  
Given an inventory containing Mature Cheese and other products, when a daily update runs, then only the exact Mature Cheese product follows these rules, and creating, reading, displaying, or searching for an item does not change it.

## Requirements

FR-001: The system shall recognize the exact product name `Mature Cheese` as an aging food product. [NEEDS CLARIFICATION: Is the official product name `Mature Cheese`, or should it be another exact name?]

FR-002: The system shall support Mature Cheese quality values from 0 through 50, inclusive.

FR-003: When Mature Cheese has `sellIn` greater than 0 at the start of an update, the system shall increase its quality by 1. [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]

FR-004: When Mature Cheese has `sellIn` equal to or less than 0 at the start of an update, the system shall increase its quality by 2. [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]

FR-005: At the end of each inventory update, the system shall decrease Mature Cheese `sellIn` by 1.

FR-006: The system shall determine the quality increase using `sellIn` before it is decreased.

FR-007: The system shall cap Mature Cheese quality at 50 after an increase. [NEEDS CLARIFICATION: Is 50 the correct maximum quality?]

FR-008: The system shall keep Mature Cheese quality between 0 and 50, inclusive, including when imported quality is outside that range; invalid values shall follow the proposed code’s existing quality normalization policy.

FR-009: The system shall apply Mature Cheese behavior only to the exact product name; similar but non-exact names shall not receive this behavior.

FR-010: The system shall leave the existing behavior of Aged Brie, Backstage passes, Sulfuras, Conjured items, and other products unchanged.

FR-011: The system shall change a Mature Cheese item’s quality or `sellIn` only during the daily inventory update, not during creation, reading, displaying, or searching.

FR-012: The system shall persist both updated quality and `sellIn` values after each daily inventory update.

FR-013: The system shall apply the rules consistently to multiple Mature Cheese items and across consecutive updates, including in mixed inventories.

FR-014: The system shall follow the approved policy for any additional behavior after a maximum age. [NEEDS CLARIFICATION: Should Mature Cheese have special behavior such as becoming unsafe or losing value after a maximum age?]

FR-015: The system shall produce the same result for the same starting item and update operation.

FR-016: The system shall keep the Mature Cheese behavior isolated and independently testable.

FR-017: The change shall modify only Proposed Code and shall not change Original Code.

FR-018: Automated unit tests shall cover the business rules and regression tests shall confirm existing product behavior is unchanged. Suggested test names are `increases_quality_by_one_before_sell_by`, `increases_quality_by_two_on_sell_by_date`, `increases_quality_by_two_after_sell_by`, `does_not_increase_quality_above_fifty`, `keeps_quality_at_fifty`, `decreases_sell_in_by_one_each_day`, and `does_not_change_existing_product_behaviour`.

FR-019: The change shall avoid unrelated refactoring or changes to unrelated product behavior.

## Success criteria

SC-001: One update produces `sellIn = 4`, `quality = 11` from `sellIn = 5`, `quality = 10`, and `sellIn = 0`, `quality = 11` from `sellIn = 1`, `quality = 10`.

SC-002: One update produces `sellIn = -1`, `quality = 12` from `sellIn = 0`, `quality = 10`, and `sellIn = -2`, `quality = 12` from `sellIn = -1`, `quality = 10`.

SC-003: Quality is 50 after an increase from 49, and remains 50 when it starts at 50; no update leaves quality outside 0–50.

SC-004: Existing product behavior and outputs remain unchanged, and accessing an item outside the daily update does not change its quality or `sellIn`.

## Acceptance criteria

AC-001 — When an item named exactly `Mature Cheese` is classified, the system shall recognize it as the new aging food product (FR-001).

AC-002 — When Mature Cheese is created or its quality is normalized, the system shall keep its quality within 0–50, inclusive (FR-002).

AC-003 — When one update starts with `sellIn = 5`, `quality = 10`, or `sellIn = 1`, `quality = 10`, the system shall increase quality by 1 (FR-003).

AC-004 — When one update starts with `sellIn = 0`, `quality = 10`, or `sellIn = -1`, `quality = 10`, the system shall increase quality by 2 (FR-004).

AC-005 — When a Mature Cheese item is updated once, the system shall decrease its `sellIn` by exactly 1 (FR-005).

AC-006 — When an update starts with `sellIn = 1`, the system shall apply the before-sell-by increase and end with `sellIn = 0` (FR-006).

AC-007 — When an increase would take quality above 50, the system shall store quality as 50 (FR-007).

AC-008 — When an update receives Mature Cheese quality below 0 or above 50, the system shall leave the updated quality within 0–50 (FR-008).

AC-009 — When an item’s name is similar to but not exactly `Mature Cheese`, the system shall not apply the Mature Cheese behavior (FR-009).

AC-010 — When existing products are updated, the system shall retain their previous behavior and outputs (FR-010).

AC-011 — When a Mature Cheese item is created, read, displayed, or searched for without a daily update, the system shall leave its quality and `sellIn` unchanged (FR-011).

AC-012 — When a daily update completes for Mature Cheese, the system shall provide the updated quality and `sellIn` values (FR-012).

AC-013 — When consecutive updates run on multiple Mature Cheese items in a mixed inventory, the system shall apply the Mature Cheese rules to each matching item on each update (FR-013).

AC-014 — When Mature Cheese reaches an age beyond its sell-by date, the system shall apply the approved maximum-age behavior (FR-014).

AC-015 — When the same starting item and update operation are repeated, the system shall produce the same result each time (FR-015).

AC-016 — When Mature Cheese rules are tested independently, the system shall allow those rules to be verified without changing other product behavior (FR-016).

AC-017 — When the feature is delivered, the system shall have changes only in Proposed Code, with Original Code unchanged (FR-017).

AC-018 — When automated tests run, the system shall have unit coverage for the business rules and regression coverage for existing products, using the suggested test names where applicable (FR-018).

AC-019 — When the feature is delivered, the system shall contain no unrelated refactoring or unrelated product behavior changes (FR-019).

## Out of scope

- Changing Original Code.
- Adding user-interface behavior.
- Introducing product behavior beyond the approved Mature Cheese rules.

## Open questions

- [NEEDS CLARIFICATION: Is the official product name `Mature Cheese`, or should it be another exact name?]
- [NEEDS CLARIFICATION: Should quality increase by 1 before sell-by and 2 afterwards?]
- [NEEDS CLARIFICATION: Is 50 the correct maximum quality?]
- [NEEDS CLARIFICATION: Should Mature Cheese have special behavior such as becoming unsafe or losing value after a maximum age?]