# Command Types Reference

Built-in commands provided by the TurnForge engine.

## Action Commands

### `MoveCommand`
Moves an agent to a target position.

```csharp
public sealed record MoveCommand(
    string AgentId,
    bool HasCost,
    Position TargetPosition
) : IActionCommand;
```

## System Commands

### `SpawnAgentsCommand`
Batch request to spawn agents.

```csharp
public sealed record SpawnAgentsCommand(
    IReadOnlyList<SpawnRequest> Requests
) : ICommand;
```

### `SpawnPropsCommand`
Batch request to spawn static props.

```csharp
public sealed record SpawnPropsCommand(
    IReadOnlyList<SpawnRequest> Requests
) : ICommand;
```

### `InitializeBoardCommand`
Sets up the initial board state.

```csharp
public sealed record InitializeBoardCommand(
    BoardDescriptor Descriptor
) : ICommand;
```

### `CommandAck`
Acknowledges completion of UI effects, allowing the engine to proceed.

```csharp
public sealed record CommandAck() : ICommand;
```
