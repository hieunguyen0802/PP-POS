# Folder structure of Domain Project

- This folder defines what business means, not how it runs

## [1] Entities

- Defines the core business object - nouns
- Example: Customer has orders; order has many OrderProduct.; Product has product inf, stock, price.

## [2] Value Objects

- Defines immutable concepts that have no identity of their own
- Example: Money has amount; address used by customer -> Enforce business rule, no negative money

## [3] Enums:

- Defines enums for entities:
- Example: PaymentMethod = cash, card, wire

# [4] Exception:

- Custom exceptions for domain-level validation and invariants
- Example: base class for domain rule violations

## [5] Events:

- Represent something that happened in the domain that others may care about
- Examples: created order events

## [6] Helpers:

- Some helper classes for more utilization
- Example: tax calculation, rounding rules

## [7] Common:

- Shared abstractions for the entities:
- Example: EntityBase, base class with Id, createdAt and updatedAt.

