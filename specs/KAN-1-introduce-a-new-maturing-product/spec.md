# Feature specification: Introduce a new maturing product (Vintage Wine)

Issue: KAN-1

UI work: no

The issue asks for a new product that matures (changes in Quality as days pass) and leaves the business rules to the team. This spec picks the product and its rules, based on the current code. `ItemKind` classifies items by name, and `ItemMaturingPolicy` holds the per-kind rules. `ItemMaturationService` applies them to an `Item`. `Quality` always stays between 0 and 50, except for the legendary Sulfuras at 80. The new product is **"Vintage Wine"**. It gains value over time, accelerates as its sell-by date nears, and never loses value.

## User scenarios

### Scenario 1: Vintage Wine matures slowly while far from its sell-by date
The innkeeper stocks Vintage Wine and ends the day. The wine gains a little value each day.

Acceptance scenarios:
1. Given a "Vintage Wine" with SellIn 20 and Quality 10, when a day passes, then it has SellIn 19 and Quality 11.
2. Given a "Vintage Wine" with SellIn 11 and Quality 10, when a day passes, then it has SellIn 10 and Quality 11.

### Scenario 2: Vintage Wine matures faster as the sell-by date approaches
The wine gains value faster once 10 days or fewer remain.

Acceptance scenarios:
1. Given a "Vintage Wine" with SellIn 10 and Quality 10, when a day passes, then it has SellIn 9 and Quality 12.
2. Given a "Vintage Wine" with SellIn 1 and Quality 10, when a day passes, then it has SellIn 0 and Quality 12.

### Scenario 3: Vintage Wine never loses value after its sell-by date
The sell-by date passes, but the wine keeps maturing at the fast rate.

Acceptance scenarios:
1. Given a "Vintage Wine" with SellIn 0 and Quality 10, when a day passes, then it has SellIn -1 and Quality 12.
2. Given a "Vintage Wine" with SellIn -5 and Quality 10, when a day passes, then it has SellIn -6 and Quality 12.

### Scenario 4: Vintage Wine never exceeds the maximum quality
Acceptance scenarios:
1. Given a "Vintage Wine" with SellIn 20 and Quality 50, when a day passes, then its Quality is still 50.
2. Given a "Vintage Wine" with SellIn 5 and Quality 49, when a day passes, then its Quality is 50.

### Scenario 5: Existing products are unaffected
Acceptance scenarios:
1. Given an "Aged Brie", "Backstage passes to a TAFKAL80ETC concert", "Conjured Mana Cake", "Sulfuras, Hand of Ragnaros" or a standard item, when a day passes, then it matures exactly as it did before this change.

## Requirements

- FR-001: The system shall recognise an item named "Vintage Wine" (or the short name "Vintage") as a distinct product kind, not as a standard item.
- FR-002: When a day passes, the system shall decrease the SellIn of a Vintage Wine by 1, including after its sell-by date has passed.
- FR-003: When a day passes and a Vintage Wine has a SellIn of 11 or more at the start of the day, the system shall increase its Quality by 1.
- FR-004: When a day passes and a Vintage Wine has a SellIn of 10 or less at the start of the day (including a SellIn of 0 or below, so after its sell-by date), the system shall increase its Quality by 2.
- FR-005: The system shall never decrease the Quality of a Vintage Wine as it matures, including after its sell-by date.
- FR-006: The system shall never let the Quality of a Vintage Wine exceed 50.
- FR-007: The system shall mature the existing kinds (Standard, Aged Brie, Backstage passes, Conjured, Sulfuras) exactly as it does today.
- FR-008: The system shall keep the item's name unchanged when it matures a Vintage Wine.

## Success criteria

- SC-001: The automated tests for the Vintage Wine rules (the boundaries at SellIn 11, 10 and 0, expired items, and the Quality cap of 50) all pass.
- SC-002: The existing tests for Standard, Aged Brie, Backstage, Conjured and Sulfuras pass without any change to their expectations.
- SC-003: The project builds with no new warnings, and the CI workflow stays green.
- SC-004: Maturing a "Vintage Wine" item through `ItemMaturationService` gives the quality and sell-in values listed in the user scenarios above.

## Acceptance criteria

- AC-001: When the name "Vintage Wine" or "Vintage" is classified, the system shall return the Vintage Wine kind and not the Standard kind (FR-001).
- AC-002: When a day passes for a Vintage Wine with SellIn 5, 0 or -5, the system shall return an item whose SellIn is 1 lower (FR-002).
- AC-003: When a day passes for a Vintage Wine with SellIn 11 or 20 and Quality 10, the system shall return Quality 11 (FR-003).
- AC-004: When a day passes for a Vintage Wine with SellIn 10, 1, 0 or -5 and Quality 10, the system shall return Quality 12 (FR-004).
- AC-005: When a day passes for a Vintage Wine with SellIn -5 and Quality 10, the system shall return a Quality of at least 10 (FR-005).
- AC-006: When a day passes for a Vintage Wine with Quality 50, or Quality 49 and SellIn 5, the system shall return Quality 50 (FR-006).
- AC-007: When a day passes for each of Aged Brie, Backstage passes, Conjured Mana Cake, Sulfuras and a standard item, the system shall return the same SellIn and Quality as before this change (FR-007).
- AC-008: When a day passes for an item named "Vintage Wine", the system shall return an item whose name is "Vintage Wine" (FR-008).

## Out of scope

- Any user interface, console output or reporting.
- Changing the rules of existing products.
- Changing the legacy `OriginalCode` (`Item`, `UpdateQualityService`). Vintage Wine is added only to the `ProposedCode` maturing flow.
- Price, stock, or any item property other than Name, SellIn and Quality.
- Matching item names by case-insensitive or partial matching. The existing exact-name matching is kept.