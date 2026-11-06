# Documentation

This repository is a C# (.NET) solution for the Gilded Rose kata. It contains the legacy item-quality code, a refactored domain-modelled implementation, and an xUnit test project for the refactored code.

## Index

- [src/Core](src-Core.md): the core class library with the original `UpdateQuality` code and the refactored `ProposedCode` (item kinds, quality and sell-in value types, maturing policy).
- [tests/Core.Tests](tests-Core.Tests.md): the xUnit tests that cover the refactored implementation (item kinds, policy, value types and the maturation service).
