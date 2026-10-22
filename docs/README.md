# Gilded Rose Kata – Documentation

This repository contains a C# (`net8.0`) solution for the Gilded Rose kata. It has one production library, `GildedRoseKata.Core`, and one xUnit test project. The library keeps the original kata `UpdateQuality` code and adds a refactored implementation that supports "Conjured" items and uses immutable value types.

## Index

| Page | Module | Purpose |
|---|---|---|
| [src-Core.md](src-Core.md) | `src/Core` | The production library. It holds the legacy `UpdateQualityService` and the refactored `ItemMaturationService` with its domain types, rules, constraints and technical debt. |
| [tests-Core.Tests.md](tests-Core.Tests.md) | `tests/Core.Tests` | The xUnit test project for the refactored code. It covers the test suites, the rules they encode, dependencies, and build and CI commands. |
