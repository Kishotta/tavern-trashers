# TavernTrashers API

A modern .NET API for tabletop role-playing game (TTRPG) utilities, focused on providing robust dice rolling mechanics and user management for gaming communities.

## 🎯 Vision

TavernTrashers aims to be the go-to API for TTRPG digital tooling, starting with two core pillars:

- **Dice Rolling**: A powerful, flexible dice expression engine supporting complex TTRPG mechanics
- **User Management**: Secure user authentication, profiles, and preferences for personalized gaming experiences

The API is designed as a modular monolith, making it easy to extend with new gaming features while maintaining clean boundaries between concerns.

## ✨ Features

### 🎲 Dice Module

The Dice module provides a comprehensive dice rolling engine with support for complex expressions:

- **Standard Dice Notation**: `d4`, `2d6`, `3d8`, etc.
- **Fate/Fudge Dice**: `df`, `2df` - rolls +1, 0, or -1
- **Keep/Drop Mechanics**: `2d20kh` (advantage), `4d6kh3` (ability scores), `3d10dl2` (drop lowest 2)
- **Exploding Dice**: `d6!`, `2d8!` - reroll on maximum values
- **Arithmetic Operations**: Full support for `+`, `-`, `*`, `/`, `%`
- **Expression Grouping**: Parentheses for complex calculations like `(2d6 + 3) * 2`
- **Roll History**: Track and retrieve past rolls (coming soon)

**Example Expressions:**
