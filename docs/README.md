# Documentation

This repository contains a solution (`GildedRoseKata.sln`) for the Gilded Rose kata. It has the legacy "update quality" code, a refactored and domain-modelled implementation that also supports Conjured items, and a unit-test project for the refactored code.

## Modules

- [src/Core](src-Core.md): the class library with the original legacy code and the proposed refactored implementation (domain types, maturing policy and service).
- [tests/Core.Tests](tests-Core.Tests.md): the xUnit test project that checks the proposed code's rules and domain types.
