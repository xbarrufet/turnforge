# Team & Controller System

## Overview

**Team** and **ControllerId** are properties on every `GameEntity` that identify faction membership and player control.

| Property | Purpose | Example |
|----------|---------|---------|
| `Team` | Faction/side for combat | "Survivors", "Zombies", "Orcs" |
| `ControllerId` | Who controls this entity | "Player1", "AI", "Player2" |

---

## When to Use

### Team (Faction)
- **Combat targeting**: `GetValidCombatTargets()` returns entities with different Team
- **Movement costs**: BarelyAlive charges extra AP when Survivors share position with Zombies team
- **Win conditions**: Check if all entities of a Team are eliminated

### ControllerId (Player)
- **Permissions**: Only the controller can issue commands for an entity
- **Turn management**: "All Player1's agents have acted"
- **Co-op games**: Multiple players controlling same Team (Zombicide)

---

## Configuration

### Option 1: In Definition (fixed per entity type)
```json
{
  "DefinitionId": "walker",
  "Name": "Walker",
  "Category": "Zombie",
  "Team": "Zombies",
  "ControllerId": "AI"
}
```

### Option 2: At Spawn (runtime override)
```csharp
new AgentDescriptor("kommando") 
{
    Team = "Orks",
    ControllerId = "Player2",
    Position = spawnPoint
}
```

**Priority**: Descriptor overrides Definition.

---

## Queries

```csharp
// Get all agents on a team
var survivors = queryService.GetAgentsByTeam("Survivors");

// Get all agents controlled by a player
var myUnits = queryService.GetAgentsByController("Player1");

// Get valid combat targets (different team, adjacent)
var enemies = queryService.GetValidCombatTargets(agentId);
```

---

## Use Cases

### Zombicide (Co-op)
```
Team: Survivors → ControllerId: Player1, Player2, Player3
Team: Zombies   → ControllerId: AI
```

### Kill Team (1v1)
```
Team: Orks    → ControllerId: Player1
Team: Marines → ControllerId: Player2
```

### 3-way Battle
```
Team: Elves   → ControllerId: Player1
Team: Orcs    → ControllerId: Player2  
Team: Undead  → ControllerId: AI
```

---

## Related Files

- [GameEntity.cs](file:///Users/barrufex/Development/TurnForge/src/TurnForge.Engine/Entities/GameEntity.cs) - Team/ControllerId properties
- [GameEntityBuildDescriptor.cs](file:///Users/barrufex/Development/TurnForge/src/TurnForge.Engine/Entities/Descriptors/GameEntityBuildDescriptor.cs) - Spawn override
- [IGameStateQuery.cs](file:///Users/barrufex/Development/TurnForge/src/TurnForge.Engine/Services/Queries/IGameStateQuery.cs) - Query methods
- [GenericActorFactory.cs](file:///Users/barrufex/Development/TurnForge/src/TurnForge.Engine/Entities/Actors/GenericActorFactory.cs) - Apply logic
