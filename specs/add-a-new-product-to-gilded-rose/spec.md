# Feature specification: Add Mature Cheese to Gilded Rose
gilded_rose_new_product_business_rules.md  
UI work: no

## User scenarios

**Daily inventory update.** As an inventory operator, I want Mature Cheese quality and sell-by values updated each day so that its aging behavior is applied consistently.

- Given Mature Cheese with `sellIn` 5 or 1 and quality 10, when the daily inventory update runs, then `sellIn` decreases by 1 and quality increases by 1.
- Given Mature Cheese with `sellIn` 0 or -1 and quality 10, when the daily inventory update runs, then `sellIn` decreases by 1 and quality increases by 2.
- Given Mature Cheese at a quality boundary, when the daily inventory update runs, then quality stays within 0–50.

**Mixed inventory.** As an inventory operator, I want the new product’s rules to apply only to Mature Cheese so other products keep their established behavior.

- Given an inventory containing Mature Cheese and existing products, when the daily inventory update runs, then each item follows its own product rules.
- Given a product with a name similar to but not exactly `Mature Cheese`, when the daily inventory update runs, then it does not receive Mature Cheese behavior.

**Read and inspect inventory.** As an inventory user, I want to create, read, display, and search for Mature Cheese without changing its values so that only a daily update changes inventory state.

- Given a Mature Cheese item, when it is created, read, displayed, or searched for, then its quality and `sellIn` remain unchanged.

## Requirements

- **FR-001** The system shall decrease Mature Cheese `sellIn` by 1 during each daily inventory update.
- **FR-002** When Mature Cheese `sellIn` is greater than 0 before an update, the system shall increase its quality by 1 for that update. [NEEDS CLARIFICATION: Confirm that the proposed increase is 1 before the sell-by date.]
- **FR-003** When Mature Cheese `sellIn` is 0 or less before an update, the system shall increase its quality by 2 for that update. [NEEDS CLARIFICATION: Confirm that the proposed increase is 2 on and after the sell-by date.]
- **FR-004** The system shall cap Mature Cheese quality at 50, including when quality is 49 or already 50 before the update. [NEEDS CLARIFICATION: Confirm that 50 is the maximum quality.]
- **FR-005** The system shall keep Mature Cheese quality between 0 and 50, including when existing inventory data has an out-of-range quality; invalid values shall follow the Proposed Code’s established data-validation policy.
- **FR-006** The system shall apply Mature Cheese behavior only to the exact product name `Mature Cheese`; similar names shall follow their own existing rules. [NEEDS CLARIFICATION: Confirm that `Mature Cheese` is the official exact product name.]
- **FR-007** The system shall leave existing product behavior unchanged, including Aged Brie, Backstage passes, Sulfuras, and Conjured items, and shall make this change only in Proposed Code, not Original Code.
- **FR-008** The system shall not change a Mature Cheese item’s quality or `sellIn` when it is created, read, displayed, or searched; those values change only during the daily inventory update.
- **FR-009** The system shall apply the specified behavior consistently across multiple Mature Cheese items, consecutive daily updates, and mixed inventories.
- **FR-010** The system shall produce the same result for the same starting item and daily update, and the Mature Cheese rules shall be verifiable with automated unit tests.

## Success criteria

- **SC-001** A daily update produces the specified quality and `sellIn` values for `sellIn` 5, 1, 0, and -1 with quality 10.
- **SC-002** Daily updates keep quality within 0–50, including starting qualities 0, 1, 49, 50, and out-of-range imported values.
- **SC-003** Automated unit tests cover the Mature Cheese rules, multiple updates and items, mixed inventories, similar names, and existing product behavior.
- **SC-004** Original Code remains unchanged.

## Acceptance criteria

- **AC-001 (FR-001):** When a Mature Cheese item is updated once, the system shall decrease `sellIn` by 1, including on each of multiple consecutive updates.
- **AC-002 (FR-002):** When Mature Cheese starts an update with `sellIn` 5 or 1 and quality 10, the system shall produce respectively `sellIn` 4 and 0, with quality 11. [NEEDS CLARIFICATION: Confirm that the proposed increase is 1 before the sell-by date.]
- **AC-003 (FR-003):** When Mature Cheese starts an update with `sellIn` 0 or -1 and quality 10, the system shall produce respectively `sellIn` -1 and -2, with quality 12. [NEEDS CLARIFICATION: Confirm that the proposed increase is 2 on and after the sell-by date.]
- **AC-004 (FR-004):** When Mature Cheese starts an update at quality 49 or 50, the system shall produce quality 50 and never exceed it. [NEEDS CLARIFICATION: Confirm that 50 is the maximum quality.]
- **AC-005 (FR-005):** When Mature Cheese starts an update with quality below 0 or above 50, the system shall follow the established validation policy and produce a quality within 0–50.
- **AC-006 (FR-006):** When an item named exactly `Mature Cheese` is updated, the system shall apply the specified Mature Cheese rules; when a similarly named item is updated, the system shall not apply those rules. [NEEDS CLARIFICATION: Confirm that `Mature Cheese` is the official exact product name.]
- **AC-007 (FR-007):** When a mixed inventory containing Mature Cheese and existing products is updated, the system shall apply the new rules to Mature Cheese while preserving the existing products’ behavior; Original Code shall remain unchanged.
- **AC-008 (FR-008):** When a Mature Cheese item is created, read, displayed, or searched for, the system shall leave its quality and `sellIn` unchanged.
- **AC-009 (FR-009):** When an inventory contains multiple Mature Cheese items and is updated over consecutive days, the system shall apply the specified daily rules to each item.
- **AC-010 (FR-010):** When the same starting item is subjected to the same daily update, the system shall produce the same result, and automated unit tests shall verify the specified Mature Cheese behavior.

## Out of scope

- Changing Original Code or refactoring unrelated product behavior.
- Adding behavior beyond the stated daily quality and sell-by rules.

## Open questions

- [NEEDS CLARIFICATION: Confirm that `Mature Cheese` is the official exact product name.]
- [NEEDS CLARIFICATION: Confirm that the proposed quality increase is 1 before the sell-by date and 2 on and after it.]
- [NEEDS CLARIFICATION: Confirm that 50 is the maximum quality.]
- [NEEDS CLARIFICATION: Should Mature Cheese have additional behavior after a maximum age, such as becoming unsafe or losing value?]