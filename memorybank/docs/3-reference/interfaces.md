# Core Interfaces Reference

## Command System

| Interface | Description |
|-----------|-------------|
| `ICommand` | Base interface for all commands. Must define `CommandType`. |
| `ICommandHandler<T>` | Handles logic for a specific command type; returns decisions. |
| `IActionCommand` | Specialization for gameplay actions (Move, Attack). |
| `IActionContext` | Provides immutable state and board to strategies. |

```csharp
public interface ICommand 
{ 
    Type CommandType { get; } 
}

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    CommandResult Handle(TCommand command);
}
```

## Decision System

| Interface | Description |
|-----------|-------------|
| `IDecision` | Base instruction for state mutation. |
| `IApplier<T>` | Executes a decision, mutating state and generating effects. |

```csharp
public interface IDecision 
{ 
    DecisionTiming Timing { get; } 
}

public interface IApplier<TDecision> where TDecision : IDecision
{
    ApplierResponse Apply(TDecision decision, GameState state);
}
```

## Entity System

| Interface | Description |
|-----------|-------------|
| `IGameEntityComponent` | Marker interface for entity components. |
| `IGameEntityDescriptor` | DTO for entity creation parameters. |
| `ISpawnStrategy<T>` | Logic for processing spawn descriptors. |
| `ISpawnApplier<T>` | Logic for executing spawn decisions. |

```csharp
public interface IGameEntityComponent { }

public interface ISpawnStrategy<TDescriptor>
{
    IReadOnlyList<SpawnDecision<TDescriptor>> Process(
        IReadOnlyList<SpawnRequest> requests, 
        GameState state
    );
}
```

## Services

| Interface | Description |
|-----------|-------------|
| `IGameStateQuery` | Read-only access to agents/props/state. |
| `ISpatialModel` | Board geometry and validation rules. |

```csharp
public interface IGameStateQuery
{
    Agent? GetAgent(string agentId);
    bool IsPositionOccupied(Position position);
}
```
