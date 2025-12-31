# Spawn System

The TurnForge spawn system handles runtime entity creation during gameplay (e.g., spawning zombies at end of round, enemies from triggers).

## Overview

```
┌────────────────────────────────────────────────────────────┐
│                    GAME DEVELOPER                           │
│                                                              │
│  1. Define entity templates (Definitions)                   │
│  2. Implement ISpawnRule for each spawn trigger             │
│  3. Register rules in mission/game config                   │
└──────────────────────────┬─────────────────────────────────┘
                           │
                           ▼
┌────────────────────────────────────────────────────────────┐
│                    ENGINE (TurnForge)                       │
│                                                              │
│  SpawnOrchestrator evaluates rules and creates entities    │
└────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. ISpawnRule (Game Developer Implements)

```csharp
public interface ISpawnRule
{
    string RuleId { get; }
    bool ShouldTrigger(GameStateView state);
    IEnumerable<SpawnInstruction> GetInstructions(GameStateView state);
}
```

**Responsibilities:**
- `ShouldTrigger`: Determine if spawn should occur (e.g., end of round, noise token)
- `GetInstructions`: Generate spawn instructions (what, where, how many)

### 2. SpawnInstruction (DTO)

```csharp
public readonly record struct SpawnInstruction
{
    public required BaseGameEntityDefinition Definition { get; init; }
    public required IBoardPosition Position { get; init; }
    public int Count { get; init; } = 1;
}
```

### 3. SpawnOrchestrator (Engine)

```csharp
var orchestrator = new SpawnOrchestrator(entityFactory);
orchestrator.RegisterRule(new EndOfRoundSpawn());
orchestrator.RegisterRule(new NoiseTriggerSpawn());

// At appropriate game phase:
orchestrator.ExecuteSpawns(stateView, overlay);
```

## Implementation Guide

### Step 1: Create Entity Definition

```csharp
public class WalkerDefinition : AgentDefinition
{
    public WalkerDefinition() : base("zombie-walker", "Zombie")
    {
        AddTrait(new HealthTrait(1));
        AddTrait(new MovementTrait(1));
        AddTrait(new AttackTrait(1));
    }
}
```

### Step 2: Implement Spawn Rule

```csharp
public class EndOfRoundZombieSpawn : ISpawnRule
{
    public string RuleId => "zombicide-end-round";
    
    public bool ShouldTrigger(GameStateView state)
    {
        // Check if we're at end of round phase
        return state.TryGet<string>("Phase", out var phase) 
            && phase == "EndRound";
    }
    
    public IEnumerable<SpawnInstruction> GetInstructions(GameStateView state)
    {
        // Get all spawn points on the board
        var spawnPoints = GetActiveSpawnPoints(state);
        
        foreach (var spawnPoint in spawnPoints)
        {
            // Determine zombie type based on threat level
            var definition = GetZombieForThreatLevel(state);
            
            yield return new SpawnInstruction
            {
                Definition = definition,
                Position = spawnPoint.Position,
                Count = spawnPoint.SpawnCount
            };
        }
    }
}
```

### Step 3: Register Rules in Mission

```csharp
public class Mission01 
{
    public IReadOnlyList<ISpawnRule> SpawnRules { get; } = new List<ISpawnRule>
    {
        new EndOfRoundZombieSpawn(),
        new NoiseTriggerSpawn(),
        new ObjectiveActivatedBossSpawn()
    };
}
```

### Step 4: Execute in Game Loop

```csharp
// In GameEngineRuntime or FSM node resolver
public void OnPhaseEnd(GameStateView stateView, GameStateOverlay overlay)
{
    _spawnOrchestrator.ExecuteSpawns(stateView, overlay);
}
```

## Key Points

| Concept | Responsibility |
|---------|----------------|
| **Definition** | Template for entity (stats, traits) |
| **ISpawnRule** | WHEN and WHERE to spawn |
| **SpawnInstruction** | Data transfer object |
| **SpawnOrchestrator** | Evaluates rules, creates entities |

## Spawn vs Deployment

| Spawn (Runtime) | Deployment (Setup) |
|-----------------|---------------------|
| During gameplay | Start of game |
| Rules determine position | Player chooses position |
| Definition only | Definition + Loadout |
| `ISpawnRule` | `DeploymentDescriptor` |

---

## Applier Mechanism

The Applier pattern provides a unified way to create entities and modifications.

### Entity Applier

```csharp
public interface IEntityApplier
{
    SpawnEntityOperation Apply(BaseGameEntityDefinition definition, IBoardPosition position);
    SpawnEntityOperation Apply(IGameEntityBuildDescriptor descriptor, IBoardPosition position);
}
```

**Use cases:**
- **Spawn**: `Apply(definition, position)` - runtime spawn from rules
- **Deployment**: `Apply(descriptor, position)` - player deployment with loadout

### Component Applier

```csharp
public interface IComponentApplier<TInput>
{
    IGameStateOperation Apply(EntityId target, TInput data);
}
```

**Use cases:**
- Damage application
- Inventory modifications
- Status effects

### Integration

```csharp
// SpawnOrchestrator uses IEntityApplier internally
var orchestrator = new SpawnOrchestrator(entityApplier);
orchestrator.RegisterRule(new EndOfRoundSpawn());
orchestrator.ExecuteSpawns(stateView, overlay);

// Or use applier directly for custom spawns
var operation = entityApplier.Apply(zombieDefinition, spawnPoint);
overlay.Record(operation);
```
