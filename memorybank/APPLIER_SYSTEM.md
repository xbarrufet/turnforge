# Applier System Architecture

The TurnForge Engine relies on a strict distinction between **Entity Creation (Build)** and **Component Updates (Update)** for state mutation. This separation ensures clarity and type safety within the Orchestrator loop.

## Overview

State mutations are delegated to **Appliers**. An Applier is a component that consumes a **Decision** and produces a new **GameState**.

- **Decisions**: Represent requests for state changes (e.g., "Spawn Prop", "Move Agent").
- **Appliers**: Execute the logic to apply these decisions, returning the updated state.

## Applier Constraint

An Applier is designed to work on a single granularity level:
- **Build Appliers**: Target **1 specific Entity Type** (e.g., `Prop`, `Agent`).
- **Update Appliers**: Target **1 specific Component Type** (e.g., `MovementComponent`, `HealthComponent`).

## 1. Build Decisions & Appliers

Build operations involve creating new entities and adding them to the state.

### Interface: `IBuildDecision<TEntity>`
Decisions that result in entity creation must implement `IBuildDecision<T>`.
- **Constraint**: `T` must be a `GameEntity`.
- **Payload**: Must provide an `IGameEntityDescriptor<T>` describing the entity to be created.

```csharp
public interface IBuildDecision<T> : IDecision where T : GameEntity
{
    IGameEntityDescriptor<T> Descriptor { get; }
}
```

### Interface: `IBuildApplier<TDecision, TEntity>`
Appliers that handle build decisions.
- **Input**: A `TDecision` (which is an `IBuildDecision<TEntity>`).
- **Output**: New `GameState` containing the new entity.
- **Logic**: Typically uses an `IGameEntityFactory<TEntity>` to construct the entity from the descriptor.

```csharp
public interface IBuildApplier<in TDecision, TEntity> : IApplier<TDecision>
    where TDecision : IBuildDecision<TEntity>
    where TEntity : GameEntity
{
}
```

### Example
- **Decision**: `PropSpawnDecision : IBuildDecision<Prop>` (Carries `PropDescriptor`).
- **Applier**: `PropApplier : IBuildApplier<PropSpawnDecision, Prop>`.

## 2. Update Decisions & Appliers

Update operations involve modifying the state of existing entities, specifically their components.

### Interface: `IUpdateDecision<TComponent>`
Decisions that result in component updates.
- **Constraint**: `TComponent` must be an `IGameEntityComponent`.
- **Payload**: Contains specific data required for the update (e.g., `TargetPosition` for movement).

```csharp
public interface IUpdateDecision<T> : IDecision where T : IGameEntityComponent
{
}
```

### Interface: `IUpdateApplier<TDecision, TComponent>`
Appliers that handle update decisions.
- **Input**: A `TDecision` (which is an `IUpdateDecision<TComponent>`).
- **Output**: New `GameState` with the updated component(s).
- **Logic**: Locates the entity/component in the state, applies the change, and returns the modified state.

```csharp
public interface IUpdateApplier<in TDecision, TComponent> : IApplier<TDecision>
    where TDecision : IUpdateDecision<TComponent>
    where TComponent : IGameEntityComponent
{
}
```

## Summary

| Type | Decision Interface | Applier Interface | Target | Payload |
|---|---|---|---|---|
| **Build** | `IBuildDecision<TEntity>` | `IBuildApplier<TDecision, TEntity>` | Entity (Creation) | `IGameEntityDescriptor<TEntity>` |
| **Update** | `IUpdateDecision<TComponent>` | `IUpdateApplier<TDecision, TComponent>` | Component (Mutation) | Update Data |
