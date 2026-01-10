# Querying Game State with LINQ

## Overview

TurnForge provides a fluent, LINQ-style query API for filtering and retrieving entities from `GameState`. The query system is **overlay-aware**, meaning it automatically reflects pending changes (created, modified, or destroyed entities) within the current transaction.

## Basic Usage

### Starting a Query

All queries start with the `Query()` method on `GameState`:

```csharp
var results = gameState.Query()
    .ControlledBy(playerId)
    .OfType<Agent>()
    .Execute();
```

### Core Query Methods

#### Filter by Owner

```csharp
// Get all entities controlled by a specific player
var playerEntities = state.Query()
    .ControlledBy(playerId)
    .Execute();
```

#### Filter by Type

```csharp
// Get all agents (pawns, units, etc.)
var agents = state.Query()
    .OfType<Agent>()
    .Execute();
```

#### Filter by Component

```csharp
// Get entities with a specific component
var movableEntities = state.Query()
    .WithComponent<IMovementComponent>()
    .Execute();

// With predicate
var damagedEntities = state.Query()
    .WithComponent<IHealthComponent>(h => h.CurrentHealth < h.MaxHealth / 2)
    .Execute();
```

#### Filter by Trait

```csharp
// Get entities with a specific trait
var flyingUnits = state.Query()
    .WithTrait<FlyingTrait>()
    .Execute();

// With predicate
var fastUnits = state.Query()
    .WithTrait<MovementTrait>(m => m.Speed > 5)
    .Execute();
```

#### Filter by Location

```csharp
// Get entities at positions matching a predicate
var entitiesOnTrack = state.Query()
    .AtLocation(pos => pos is TilePosition tp && tp.TileId.Value.StartsWith("track_"))
    .Execute();
```

#### Custom Filters

```csharp
// Add any custom filter
var results = state.Query()
    .Where(e => e.Team == "red" && e.Category == "warrior")
    .Execute();
```

## Common Extensions

### General Extensions

```csharp
// Get entities at a specific position
var entitiesHere = state.Query()
    .At(position)
    .Execute();

// Get entities in a team
var teamMembers = state.Query()
    .InTeam("red")
    .Execute();

// Pattern matching on location strings
var spawnEntities = state.Query()
    .WhereLocationMatches(loc => loc.Contains("spawn"))
    .Execute();
```

## Combining Filters

Filters can be chained together for complex queries:

```csharp
// Get damaged friendly agents on the battlefield
var damagedAllies = state.Query()
    .ControlledBy(currentPlayerId)
    .OfType<Agent>()
    .WithComponent<IHealthComponent>(h => h.CurrentHealth < h.MaxHealth)
    .AtLocation(pos => !pos.ToString().Contains("spawn"))
    .Execute();
```

## Overlay-Aware Behavior

The query system automatically handles overlay changes:

```csharp
// Create a new entity in the overlay
var newAgent = factory.CreateAgent(...);
overlay.Record(new CreateOperation(newAgent));

// Query will include the new entity
var allAgents = state.Query()
    .OfType<Agent>()
    .Execute(); // Includes newAgent

// Destroy an entity in the overlay
overlay.Record(new DestroyOperation(entityId));

// Query will exclude the destroyed entity
var remainingAgents = state.Query()
    .OfType<Agent>()
    .Execute(); // Does NOT include destroyed entity
```

## Game-Specific Extensions

### Example: Parchis Extensions

For Parchis, we provide location-based extensions:

```csharp
using ParchisLudo.Rules.Extensions;

// Get pawns not in spawn
var activePawns = state.Query()
    .ControlledBy(playerId)
    .NotInSpawn()
    .OfType<Agent>()
    .Execute();

// Get pawns on the main track
var trackPawns = state.Query()
    .OnTrack()
    .Execute();

// Get pawns in finish lane
var finishPawns = state.Query()
    .InFinishLane("red")
    .Execute();

// Get pawns in spawn
var spawnPawns = state.Query()
    .InSpawn()
    .Execute();

// Get pawns at home (final position)
var homePawns = state.Query()
    .InHome("red")
    .Execute();
```

### Creating Custom Extensions

You can create your own game-specific extensions:

```csharp
public static class MyGameQueryExtensions
{
    public static GameStateQuery InCombat(this GameStateQuery query)
    {
        return query.WithComponent<ICombatComponent>(c => c.IsEngaged);
    }
    
    public static GameStateQuery OnHighGround(this GameStateQuery query)
    {
        return query.AtLocation(pos => 
            pos is TilePosition tp && tp.Elevation > 0);
    }
}

// Usage
var advantagedUnits = state.Query()
    .InCombat()
    .OnHighGround()
    .Execute();
```

## Performance Considerations

### Current Implementation

The query system uses LINQ iteration over all entities. This is efficient for:
- Small to medium entity counts (< 1000 entities)
- Queries that filter by multiple criteria
- Development and prototyping

### Future Optimizations

For games with large entity counts, consider:

1. **Index-Aware Queries**: The system can be extended to use existing indexes (`PlayerEntities`, `TeamEntities`, `PositionEntities`) to reduce the search space.

2. **Query Caching**: Cache frequently-used query results and invalidate on state changes.

3. **Lazy Evaluation**: The current implementation is eager; lazy evaluation could improve performance for partial result sets.

## Best Practices

### 1. Filter Early

Place the most restrictive filters first:

```csharp
// Good - filters by player first (uses index internally)
var result = state.Query()
    .ControlledBy(playerId)  // Narrow down first
    .OfType<Agent>()
    .WithComponent<IHealthComponent>()
    .Execute();

// Less efficient - filters all entities first
var result = state.Query()
    .WithComponent<IHealthComponent>()
    .ControlledBy(playerId)
    .Execute();
```

### 2. Use Type Filters

Always use `OfType<T>()` when you know the entity type:

```csharp
// Good
var agents = state.Query()
    .OfType<Agent>()
    .Execute();

// Less efficient
var agents = state.Query()
    .Where(e => e is Agent)
    .Execute();
```

### 3. Avoid Repeated Queries

Cache query results when possible:

```csharp
// Bad - queries twice
if (state.Query().ControlledBy(playerId).Execute().Any())
{
    var entities = state.Query().ControlledBy(playerId).Execute();
    // Use entities...
}

// Good - query once
var entities = state.Query().ControlledBy(playerId).Execute().ToList();
if (entities.Any())
{
    // Use entities...
}
```

### 4. Use Game-Specific Extensions

Create and use game-specific extensions for common patterns:

```csharp
// Instead of repeating this pattern:
var pawns = state.Query()
    .AtLocation(pos => pos is TilePosition tp && !tp.TileId.Value.Contains("spawn"))
    .Execute();

// Create an extension:
public static GameStateQuery NotInSpawn(this GameStateQuery query)
{
    return query.AtLocation(pos => 
        pos is TilePosition tp && !tp.TileId.Value.Contains("spawn"));
}

// Use it:
var pawns = state.Query().NotInSpawn().Execute();
```

## Common Patterns

### Find Entities in Range

```csharp
public IEnumerable<GameEntity> GetEntitiesInRange(
    GameState state, 
    IBoardPosition center, 
    int range)
{
    return state.Query()
        .AtLocation(pos => CalculateDistance(center, pos) <= range)
        .Execute();
}
```

### Find Capturable Enemies

```csharp
public IEnumerable<Agent> GetCapturableEnemies(
    GameState state,
    PlayerId currentPlayer,
    IBoardPosition position)
{
    return state.Query()
        .Where(e => e.PlayerId != currentPlayer)
        .OfType<Agent>()
        .At(position)
        .Execute()
        .Cast<Agent>();
}
```

### Find Available Moves

```csharp
public IEnumerable<IBoardPosition> GetAvailableDestinations(
    GameState state,
    EntityId entityId)
{
    var entity = state.GetOverlayedEntity(entityId);
    var movement = entity.GetComponent<IMovementComponent>();
    
    if (movement == null) return Enumerable.Empty<IBoardPosition>();
    
    return state.Board.GetReachablePositions(
        entity.CurrentPosition, 
        movement.Range);
}
```

## API Reference

### GameStateQuery Methods

| Method | Description |
|--------|-------------|
| `ControlledBy(PlayerId)` | Filter by entity owner |
| `WithComponent<T>(predicate?)` | Filter by component presence/predicate |
| `WithTrait<T>(predicate?)` | Filter by trait presence/predicate |
| `AtLocation(predicate)` | Filter by position predicate |
| `OfType<T>()` | Filter by entity type |
| `Where(predicate)` | Custom filter |
| `Execute()` | Execute query and return results |

### General Extensions

| Extension | Description |
|-----------|-------------|
| `At(position)` | Get entities at specific position |
| `InTeam(team)` | Get entities in team |
| `WhereLocationMatches(pattern)` | Pattern matching on position strings |

### Parchis Extensions

| Extension | Description |
|-----------|-------------|
| `NotInSpawn()` | Pawns not in spawn |
| `OnTrack()` | Pawns on main track |
| `InFinishLane(color)` | Pawns in finish lane |
| `InSpawn()` | Pawns in spawn |
| `InHome(color)` | Pawns at home position |
