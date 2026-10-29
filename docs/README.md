# Documentation

This repository holds the Gilded Rose kata in .NET (`net8.0`). It has a core library with the legacy implementation and a refactored, domain-modelled one. It also has an xUnit test project for the refactored code.

## Index

- [src/Core](src-Core.md): the `GildedRoseKata.Core` library. It holds the legacy `OriginalCode` and the refactored `ProposedCode` that update item quality at end of day.
- [tests/Core.Tests](tests-Core.Tests.md): the xUnit tests for `ProposedCode`. They cover the maturation service, item kinds, the maturing policy, `Quality` and `SellIn`.
