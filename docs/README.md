# Documentation

This repository is a C# (.NET) solution for the Gilded Rose kata. The task is to update the `SellIn` and `Quality` of inn items each day. The solution has a core library with a legacy and a refactored implementation of the rules, and an xUnit test project for the refactored code.

## Modules

- [src/Core](src-Core.md): the production library. It holds the legacy `OriginalCode` and the refactored, non-mutating `ProposedCode`.
- [tests/Core.Tests](tests-Core.Tests.md): the xUnit and FluentAssertions tests for `ProposedCode`: value types, item kinds, maturing policy and service.
