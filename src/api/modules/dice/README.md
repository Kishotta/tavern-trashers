# Dice Module

The Dice module provides random dice rolling capabilities for players and NPCs. It supports standard polyhedral dice (d4, d6, d8, d10, d12, d20, etc.), Fate/Fudge dice, and groups of dice with modifiers.

## Domain

- **Roll** – a value object representing a die roll (size, number of dice, modifier).
- **RollResult** – returns the individual roll results and total.

## Application

Application services expose commands and queries for rolling dice. They use the result pattern to return success or failure.

## Infrastructure

Dice-specific infrastructure is minimal because rolling is deterministic; but this layer defines seeding and randomization options for reproducible tests.

## Presentation

Minimal API endpoints are provided at `/api/dice/roll`:

```
POST /api/dice/roll
{
  "dice": "2d6+3"
}
```

The response returns the individual roll results and total.

## Tests

Unit tests ensure that the randomization distribution is uniform and that parsing of dice notation is case-insensitive.
