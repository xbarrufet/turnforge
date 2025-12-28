# Creating a Game: End-to-End Tutorial

Build a simple turn-based game from scratch using TurnForge.

---

## What We'll Build

A mini-game where:
- Player controls a Survivor
- Zombies spawn and move toward player
- Player can attack zombies
- Traps damage anyone stepping on them

---

## Step 1: Project Setup

```bash
dotnet new console -n MyZombieGame
cd MyZombieGame
dotnet add reference ../TurnForge.Engine/TurnForge.Engine.csproj
```

---

## Step 2: Define Entities

```csharp
// Definitions/EntityDefinitions.cs
public static class EntityDefinitions
{
    public static void RegisterAll(IGameCatalog catalog)
    {
        // Survivor
        catalog.RegisterDefinition(
            new BaseGameEntityDefinition("Survivor", "Player")
                .AddTrait(new HealthTrait(10))
                .AddTrait(new MovementTrait(3))
                .AddTrait(new AttackTrait(2, Range: 1))
        );
        
        // Zombie
        catalog.RegisterDefinition(
            new BaseGameEntityDefinition("Zombie", "Enemy")
                .AddTrait(new HealthTrait(3))
                .AddTrait(new MovementTrait(1))
                .AddTrait(new AttackTrait(1, Range: 1))
        );
        
        // Trap
        catalog.RegisterDefinition(
            new BaseGameEntityDefinition("Trap", "Hazard")
                .AddTrait(new ExplodeOnWalkOverTrait { Damage = 5 })
        );
    }
}
```

---

## Step 3: Create Custom Traits

```csharp
// Traits/ExplodeOnWalkOverTrait.cs
public class ExplodeOnWalkOverTrait : IBaseTrait
{
    public int Damage { get; init; } = 5;
}
```

---

## Step 4: Define Events

```csharp
// Events/GameEvents.cs
public record MovedToEvent(EntityId AgentId, Position NewPosition) : IWorkflowEvent;
public record AttackResolvedEvent(EntityId Attacker, EntityId Target, int Damage) : IWorkflowEvent;
```

---

## Step 5: Create Move Workflow

```csharp
// Workflows/MoveWorkflow.cs
public class MoveWorkflow : IWorkflow
{
    public WorkflowId Id { get; } = new("Game.Move");
    public INode StartNode { get; }
    public IReadOnlyCollection<IReaction> GlobalReactions { get; }
    
    public MoveWorkflow()
    {
        var validation = new MoveValidationNode();
        var execution = new MoveExecutionNode();
        
        validation.NextNode = execution;
        StartNode = validation;
        
        GlobalReactions = new List<IReaction>
        {
            new TrapReaction()
        };
    }
    
    // ... GetNode implementation
}
```

---

## Step 6: Create Trap Reaction

```csharp
// Reactions/TrapReaction.cs
public class TrapReaction : IReaction
{
    public ReactionId Id { get; } = new("Game.Trap");

    public bool CanReact(WorkflowContext context)
    {
        var events = context.PendingEvents.OfType<MovedToEvent>();
        var state = context.GetProjectedState();
        
        return events.Any(e => 
            state.GetEntitiesAt(e.NewPosition)
                 .Any(ent => ent.HasTrait<ExplodeOnWalkOverTrait>()));
    }

    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        var state = context.GetProjectedState();
        
        foreach (var evt in context.PendingEvents.OfType<MovedToEvent>())
        {
            var traps = state.GetEntitiesAt(evt.NewPosition)
                             .Where(e => e.HasTrait<ExplodeOnWalkOverTrait>());
            
            foreach (var trap in traps)
            {
                var damage = trap.GetTrait<ExplodeOnWalkOverTrait>()!.Damage;
                context.RecordDecision(new DamageDecision(evt.AgentId, damage));
                context.RecordDecision(new DestroyEntityDecision(trap.Id));
            }
        }
        
        return ReactionResult.Continue();
    }
}
```

---

## Step 7: Define FSM Phases

```csharp
// Fsm/GamePhases.cs
public static class GamePhases
{
    public static FsmGraph BuildGraph()
    {
        var graph = new FsmGraph();
        
        graph.AddPhase("PlayerTurn", new PlayerTurnNode());
        graph.AddPhase("ZombieTurn", new ZombieTurnNode());
        graph.AddPhase("EndCheck", new EndCheckNode());
        
        graph.AddTransition("PlayerTurn", "ZombieTurn", "turnEnded");
        graph.AddTransition("ZombieTurn", "EndCheck", "turnEnded");
        graph.AddTransition("EndCheck", "PlayerTurn", "continue");
        graph.AddTransition("EndCheck", "GameOver", "victory");
        graph.AddTransition("EndCheck", "GameOver", "defeat");
        
        return graph;
    }
}
```

---

## Step 8: Wire Up Engine

```csharp
// Program.cs
var catalog = new InMemoryGameCatalog();
EntityDefinitions.RegisterAll(catalog);

var engine = GameEngineFactory.Create(config =>
{
    config.WithCatalog(catalog);
    config.WithWorkflowOrchestrator(new WorkflowOrchestrator());
    config.WithFsm(GamePhases.BuildGraph());
});

// Initialize board
engine.SendCommand(new InitializeBoardCommand(20, 15));

// Spawn survivor
engine.SendCommand(new SpawnAgentsCommand(new[]
{
    new SpawnRequest("Survivor") { Position = new Position(5, 5) }
}));

// Spawn zombies
engine.SendCommand(new SpawnAgentsCommand(new[]
{
    new SpawnRequest("Zombie") { Position = new Position(15, 10), Count = 3 }
}));

// Game loop
while (!engine.IsGameOver)
{
    var input = GetPlayerInput();
    engine.SendCommand(input);
    engine.Step();
}
```

---

## Summary

| Component | File | Purpose |
|-----------|------|---------|
| Traits | `Traits/*.cs` | Define entity capabilities |
| Definitions | `Definitions/*.cs` | Register entity types |
| Events | `Events/*.cs` | Signal workflow actions |
| Workflows | `Workflows/*.cs` | Define action pipelines |
| Reactions | `Reactions/*.cs` | Implement game rules |
| FSM | `Fsm/*.cs` | Control game flow |

---

## Next Steps

1. Add AttackWorkflow
2. Add ZombieAI for enemy turns
3. Add victory/defeat conditions
4. Connect to UI
