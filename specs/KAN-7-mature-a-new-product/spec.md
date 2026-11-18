# Feature specification: Mature Cheese, a new product with its own maturing rule

UI work: no

Issue: KAN-7

## User scenarios

### Scenario 1: The inventory recognises Mature Cheese
A shop keeper adds Mature Cheese to the inventory. They want the system to recognise it by name, so it isn't handled like a standard item.
- Given an item named "Mature Cheese", When the system matures the item for one day, Then the system applies the Mature Cheese rule and not the standard rule.
- Given an item named "MatureCheese", When the system matures the item for one day, Then the system applies the Mature Cheese rule and not the standard rule.

### Scenario 2: Mature Cheese improves as it ages
A shop keeper runs the end-of-day update. They want Mature Cheese to gain Quality each day, and to gain it faster once the sell-by date has passed.
- Given a Mature Cheese with SellIn above 0 and Quality below 50, When one day passes, Then SellIn decreases by 1 and Quality increases by 1.
- Given a Mature Cheese with SellIn of 0 or below and Quality below 50, When one day passes, Then SellIn decreases by 1 and Quality increases by 2, up to a maximum of 50.
- Given a Mature Cheese with Quality of 50, When one day passes, Then Quality stays at 50.
- Given a Mature Cheese with Quality of 49 and SellIn of 0 or below, When one day passes, Then Quality becomes 50 and not 51.
- Given a Mature Cheese, When one day passes, Then its Quality never decreases.

### Scenario 3: Existing products are unchanged
A shop keeper keeps selling the existing products. They want nothing about them to change.
- Given a Standard, Aged Brie, Backstage pass, Conjured or Sulfuras item, When one day passes, Then its Quality and SellIn change exactly as they did before this change.

## Requirements

- FR-001: The system shall support a new product kind, Mature Cheese, in addition to Standard, Aged Brie, Backstage, Conjured and Sulfuras.
- FR-002: The system shall recognise an item as Mature Cheese when its name is "Mature Cheese" or "MatureCheese".
- FR-003: The system shall mature Mature Cheese by its own rule, separate from the rules of the other kinds. Quality increases by 1 per day while SellIn is above 0, and by 2 per day when SellIn is 0 or below. Quality never decreases.
- FR-004: When Mature Cheese matures for one day, the system shall decrease its SellIn by 1.
- FR-005: The system shall keep the Quality of Mature Cheese between 0 and 50 after maturing. Quality stops increasing at 50, and Mature Cheese is not a special case like Sulfuras.
- FR-006: The system shall keep the name of Mature Cheese unchanged when it matures.
- FR-007: The system shall mature Standard, Aged Brie, Backstage, Conjured and Sulfuras items exactly as it did before this change.

## Success criteria

- SC-001: A "Mature Cheese" item and a "MatureCheese" item each mature differently from a Standard item with the same Quality and SellIn. For example, with Quality 10 and SellIn 5, Mature Cheese goes to Quality 11 and a Standard item goes to Quality 9.
- SC-002: Automated tests for the Mature Cheese rule pass. They cover SellIn above 0, SellIn of 0, SellIn below 0, and the Quality limit of 50.
- SC-003: All existing tests for the other kinds pass without changes.

## Acceptance criteria

- AC-001: When the name of an item is "Mature Cheese" or "MatureCheese", the system shall treat the item as the Mature Cheese kind (FR-001, FR-002).
- AC-002: When a Mature Cheese with SellIn above 0 and Quality below 50 matures for one day, the system shall return a Quality that is 1 higher than before. When its SellIn is 0 or below and its Quality is below 49, the system shall return a Quality that is 2 higher than before. In both cases the system shall not apply the rule of any other kind (FR-003).
- AC-003: When a Mature Cheese matures for one day, the system shall return a SellIn that is 1 lower than before (FR-004).
- AC-004: When a Mature Cheese matures, the system shall return a Quality of at least 0 and at most 50, including when its Quality is 49 or 50 before maturing (FR-005).
- AC-005: When a Mature Cheese matures, the system shall return an item with the same name (FR-006).
- AC-006: When a Standard, Aged Brie, Backstage, Conjured or Sulfuras item matures, the system shall return the same Quality and SellIn as before this change (FR-007).

## Out of scope

- Changes to the rules of Standard, Aged Brie, Backstage, Conjured and Sulfuras items.
- Changes to the `Item` class or to the original `UpdateQualityService`.
- A user interface, a product catalogue, or a way to define rules at run time.
- Name aliases for Mature Cheese other than "Mature Cheese" and "MatureCheese".