# TurnForge Examples

Practical implementations demonstrating TurnForge patterns.

---

## Available Examples

### 🎲 [Parchís Implementation Guide](parchis_implementation_guide.md)

A complete Parchís (Ludo) game demonstrating:
- Board topology (circuit + finish lanes)
- FSM phases (RollDice → MovePiece → CheckVictory → NextPlayer)
- Workflows (RollDiceWorkflow, MovePieceWorkflow)
- Reactions (CaptureReaction, SafeZoneReaction)
- TurnForge value analysis (85% time savings)

**Complexity:** ⭐⭐⭐ (~700 lines)

---

### ⭕ [TicTacToe Implementation Guide](tictactoe_implementation_guide.md)

A minimal implementation demonstrating:
- GameState.Metadata for all state
- WorkflowOrchestrator execution
- Workflow nodes (Validate → PlaceMark → CheckResult → SwitchPlayer)
- Typed workflow data (IWorkflowData)
- Design decision rationale

**Complexity:** ⭐ (~280 lines)

---

## Quick Comparison

| Aspect | TicTacToe | Parchís |
|--------|-----------|---------|
| Lines of code | ~280 | ~700 |
| Files | 2 | 8 |
| Workflow nodes | 4 | 6 |
| Reactions | 0 | 2 |
| FSM phases | 3 | 4 |
| Board topology | None | 93 tiles |
| Implementation time | ~30 min | ~2.5h |

---

## Running Examples

```bash
# TicTacToe simulation
dotnet run --project tests/TicTacToe.Simulation

# Parchís simulation
dotnet run --project tests/Parchis.Simulation
```

---

## Projects Structure

```
src/
├── TicTacToe.Rules/         # TicTacToe game rules
└── Parchis.Rules/           # Parchís game rules

tests/
├── TicTacToe.Simulation/    # TicTacToe console demo
└── Parchis.Simulation/      # Parchís console demo
```
