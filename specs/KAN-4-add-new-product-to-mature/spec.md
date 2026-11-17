# Feature specification: Add Bordeaux Wine as a new maturing product

Issue: KAN-4

Context: `ItemMaturationService` in `src/Core/ProposedCode` is the new service. It recognises products by exact name (`ItemKind.FromName`) and applies a maturing rule per product kind. Any name it does not know is treated as a normal item that loses quality. Aged Brie already gains quality as it ages, with +1 per day and +2 once sell-in is below 0. Quality is limited to 0–50. The legacy `OriginalCode/UpdateQualityService.cs` is a separate service.

## User scenarios

### Scenario 1: Bordeaux Wine gains quality while within its sell-by period
A shop keeper runs the daily update. Bordeaux Wine should become more valuable as it ages, not less.

Acceptance scenarios:
- Given an item named `Bordeaux Wine` with sell-in 10 and quality 20, when a day is matured, then its quality is 21 and its sell-in is 9.
- Given an item named `Bordeaux Wine` with sell-in 1 and quality 20, when a day is matured, then its quality is 21 and its sell-in is 0.

### Scenario 2: Bordeaux Wine matures faster after its sell-by date
Given a sell-by date that has passed, the wine gains value twice as fast.

Acceptance scenarios:
- Given an item named `Bordeaux Wine` with sell-in 0 and quality 20, when a day is matured, then its quality is 22 and its sell-in is -1.
- Given an item named `Bordeaux Wine` with sell-in -5 and quality 20, when a day is matured, then its quality is 22 and its sell-in is -6.

### Scenario 3: Quality stays within its limits
Acceptance scenarios:
- Given `Bordeaux Wine` with quality 50, when a day is matured, then its quality is still 50.
- Given `Bordeaux Wine` with sell-in 10 and quality 49 or sell-in below 0 and quality 49, when a day is matured, then its quality is 50.
- Given `Bordeaux Wine` with quality 0, when a day is matured, then its quality is never negative. It is 1 within the sell-by period and 2 after it.

### Scenario 4: Other products and the legacy service are unaffected
Acceptance scenarios:
- Given any other item (Standard, Aged Brie, Backstage passes, Conjured, Sulfuras), when a day is matured, then the result is the same as before this change.
- Given the legacy file `OriginalCode/UpdateQualityService.cs`, when the change is delivered, then that file is unchanged.

## Requirements

- FR-001: The system shall recognise the product named exactly `Bordeaux Wine` as a new maturing product.
- FR-002: While the product is within its sell-by period, the system shall increase the quality of `Bordeaux Wine` by 1 point per day.
- FR-003: Once the sell-in value is below 0, the system shall increase the quality of `Bordeaux Wine` by 2 points per day.
- FR-004: The system shall never let the quality of `Bordeaux Wine` exceed 50.
- FR-005: The system shall never let the quality of `Bordeaux Wine` be negative.
- FR-006: The system shall decrease the sell-in of `Bordeaux Wine` by 1 each day, as for other products.
- FR-007: The system shall never treat `Bordeaux Wine` as a normal item that loses quality over time.
- FR-008: The system shall leave the maturation of all other products unchanged.
- FR-009: All Bordeaux Wine maturation rules shall be implemented in the new service (`ItemMaturationService`).
- FR-010: The legacy file `OriginalCode/UpdateQualityService.cs` shall remain completely unchanged, and shall not handle `Bordeaux Wine`.
- FR-011: Automated tests shall cover the Bordeaux Wine rules above, including the sell-by boundary (sell-in 0 becoming -1) and the quality limits of 0 and 50.

## Success criteria

- SC-001: Maturing `Bordeaux Wine` with sell-in ≥ 1 raises quality by exactly 1, up to a maximum of 50.
- SC-002: Maturing `Bordeaux Wine` with sell-in ≤ 0 raises quality by exactly 2, up to a maximum of 50.
- SC-003: No Bordeaux Wine quality value ever falls outside 0–50.
- SC-004: The existing tests for the other products still pass without modification.
- SC-005: `git diff` shows no change to `src/Core/OriginalCode/UpdateQualityService.cs`.
- SC-006: The new tests for Bordeaux Wine pass in the build.

## Out of scope

- Changing the legacy `OriginalCode/UpdateQualityService.cs` or making it support `Bordeaux Wine`.
- Adding `Cellar-Aged Mead` (KAN-1) or any other product.
- Changing the rules of existing products.
- Recognising name variants or aliases of `Bordeaux Wine` (only the exact name is in scope).
- Changing the 0–50 quality limits.