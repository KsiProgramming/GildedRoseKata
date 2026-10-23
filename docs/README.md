# Documentation

This repository is a C# (.NET 8) implementation of the Gilded Rose kata. It has two versions of the inventory-aging rules: the legacy `UpdateQuality` code and a refactored, domain-modelled version. An xUnit test project covers the refactored version.

## Module index

| Page | Module | Purpose |
|---|---|---|
| [src-Core.md](src-Core.md) | `src/Core` | The `GildedRoseKata.Core` library. It holds the original `UpdateQuality` code and the refactored `ProposedCode` domain model, policy and service. |
| [tests-Core.Tests.md](tests-Core.Tests.md) | `tests/Core.Tests` | The xUnit test project. It checks the `ProposedCode` types and runs in CI. |
