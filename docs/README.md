# Documentation

This repository is a C# solution for the Gilded Rose kata. It has a legacy inventory-update implementation and a refactored one. The refactored version uses small domain types and a policy per item kind, and it adds the "Conjured" rule. The tests cover the refactored code.

## Index

- [src/Core](src-Core.md): the class library with the original and the refactored (`ProposedCode`) implementations of the end-of-day inventory update.
- [tests/Core.Tests](tests-Core.Tests.md): the xUnit test project for the `ProposedCode` domain model and maturation service.
