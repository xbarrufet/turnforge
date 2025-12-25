# Decision Types Reference

Built-in decisions representing intent to mutate state.

## Core Decisions

### `ActionDecision`
General-purpose entity update. Modifies components on an existing entity.

```csharp
public sealed record ActionDecision : IDecision
{
    public string EntityId { get; init; }
    public IReadOnlyDictionary<Type, IGameEntityComponent> ComponentUpdates { get; init; }
    public DecisionTiming Timing { get; init; }
}
```

### `SpawnDecision<T>`
Instruction to create a new entity.

```csharp
public sealed record SpawnDecision<TDesc> : IDecision 
    where TDesc : IGameEntityDescriptor
{
    public TDesc Descriptor { get; init; }
    public DecisionTiming Timing { get; init; }
}
```

### `ChangeStateDecision`
Instruction to transition the FSM to a new state.

```csharp
public sealed record ChangeStateDecision(
    string TargetStateId, 
    string TransitionTag
) : IDecision;
```
