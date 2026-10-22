# Documentation

This repository holds a C# solution for the Gilded Rose kata (`GildedRoseKata.sln`). It contains the original legacy "update quality" code, a refactored domain-modelled version (ProposedCode), and a test project for the refactored version.

## Index

- [src/Core](src-Core.md): the core library with the legacy `UpdateQualityService` and the refactored `ItemMaturationService`, its policy and domain types.
- [tests/Core.Tests](tests-Core.Tests.md): the xUnit test project that covers the refactored ProposedCode (item kinds, maturing rules, quality, sell-in and the service).
