# Event-Driven Reaction System Architecture

## Overview

This document describes a proposed event-driven reaction system for TurnForge that enables entities to react to game events in a controlled, sequential manner.

## Requirements

- **Event Emission**: Actions emit events when operations occur (move, damage, etc.)
- **Event Reactions**: Entities can react to events and emit new events
- **Controlled Execution**: Events are processed sequentially with safeguards against infinite loops
- **Transactional**: All reactions occur within the overlay system
- **UI-Friendly**: Event history is available for animations

## Core Concepts

### 1. Game Events (`IGameEvent`)

Events represent things that happen in the game. They are immutable records of state changes.

```csharp
public interface IGameEvent
{
    EventId Id { get; }
    EntityId? SourceEntityId { get; }
    string EventType { get; }
    DateTime Timestamp { get; }
}

public record struct EventId(Guid Value)
{
    public static EventId New() => new(Guid.NewGuid());
}
```

#### Standard Event Types

```csharp
// Movement
public record EntityMovedEvent(
    EventId Id,
    EntityId EntityId,
    IBoardPosition From,
    IBoardPosition To
) : IGameEvent
{
    public EntityId? SourceEntityId => EntityId;
    public string EventType => "EntityMoved";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

// Combat
public record EntityDamagedEvent(
    EventId Id,
    EntityId VictimId,
    EntityId? AttackerId,
    int Damage
) : IGameEvent
{
    public EntityId? SourceEntityId => VictimId;
    public string EventType => "EntityDamaged";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record EntityDestroyedEvent(
    EventId Id,
    EntityId EntityId,
    EntityId? KillerId
) : IGameEvent
{
    public EntityId? SourceEntityId => EntityId;
    public string EventType => "EntityDestroyed";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

// Parchis-specific
public record EntityCapturedEvent(
    EventId Id,
    EntityId CapturedEntityId,
    EntityId CapturerEntityId,
    IBoardPosition ReturnPosition
) : IGameEvent
{
    public EntityId? SourceEntityId => CapturedEntityId;
    public string EventType => "EntityCaptured";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
```

### 2. Event Reactions (`IEventReaction`)

Reactions are objects that listen for specific events and execute logic when triggered.

```csharp
public interface IEventReaction
{
    /// <summary>
    /// Check if this reaction should trigger for the given event
    /// </summary>
    bool ShouldReact(IGameEvent gameEvent, GameStateView state);
    
    /// <summary>
    /// Execute the reaction, potentially emitting new events
    /// </summary>
    IEnumerable<IGameEvent> Execute(IGameEvent triggerEvent, GameStateView state);
}
```

#### Example: Trap Reaction

```csharp
public class TrapReaction : IEventReaction
{
    private readonly EntityId _trapEntityId;
    
    public TrapReaction(EntityId trapEntityId)
    {
        _trapEntityId = trapEntityId;
    }
    
    public bool ShouldReact(IGameEvent gameEvent, GameStateView state)
    {
        if (gameEvent is not EntityMovedEvent moved) return false;
        
        var trap = state.GetEntity(_trapEntityId);
        var trapPos = state.GetPosition(_trapEntityId);
        
        // Trigger if entity moved TO trap position
        return moved.To.Equals(trapPos);
    }
    
    public IEnumerable<IGameEvent> Execute(IGameEvent triggerEvent, GameStateView state)
    {
        var moved = (EntityMovedEvent)triggerEvent;
        
        // Apply damage
        state.RecordOperation(new DamageOperation(moved.EntityId, 2));
        
        // Emit damage event (can trigger more reactions!)
        yield return new EntityDamagedEvent(
            EventId.New(),
            moved.EntityId,
            _trapEntityId,
            2
        );
        
        // Destroy trap after use
        state.RecordOperation(new DestroyOperation(_trapEntityId));
        
        yield return new EntityDestroyedEvent(
            EventId.New(),
            _trapEntityId,
            null
        );
    }
}
```

### 3. Event Queue

Manages pending events in FIFO order.

```csharp
public class EventQueue
{
    private readonly Queue<IGameEvent> _events = new();
    private readonly List<IGameEvent> _processedEvents = new();
    
    public void Enqueue(IGameEvent gameEvent)
    {
        _events.Enqueue(gameEvent);
    }
    
    public bool HasPendingEvents => _events.Count > 0;
    
    public IGameEvent? Dequeue()
    {
        return _events.Count > 0 ? _events.Dequeue() : null;
    }
    
    public void MarkProcessed(IGameEvent gameEvent)
    {
        _processedEvents.Add(gameEvent);
    }
    
    public IReadOnlyList<IGameEvent> GetProcessedEvents() => _processedEvents;
    
    public void Clear()
    {
        _events.Clear();
        _processedEvents.Clear();
    }
}
```

### 4. Event Processor

Processes events sequentially and handles cascading reactions.

```csharp
public class EventProcessor
{
    private readonly EventQueue _eventQueue;
    private readonly List<IEventReaction> _reactions = new();
    private int _maxIterations = 100; // Prevent infinite loops
    
    public EventProcessor(EventQueue eventQueue)
    {
        _eventQueue = eventQueue;
    }
    
    public void RegisterReaction(IEventReaction reaction)
    {
        _reactions.Add(reaction);
    }
    
    /// <summary>
    /// Process all pending events and their cascading reactions
    /// </summary>
    public EventProcessingResult ProcessEvents(GameStateView state)
    {
        int iterations = 0;
        var allProcessedEvents = new List<IGameEvent>();
        
        while (_eventQueue.HasPendingEvents && iterations < _maxIterations)
        {
            var currentEvent = _eventQueue.Dequeue();
            if (currentEvent == null) break;
            
            allProcessedEvents.Add(currentEvent);
            _eventQueue.MarkProcessed(currentEvent);
            
            // Find and execute matching reactions
            foreach (var reaction in _reactions)
            {
                if (reaction.ShouldReact(currentEvent, state))
                {
                    // Execute reaction and enqueue new events
                    foreach (var newEvent in reaction.Execute(currentEvent, state))
                    {
                        _eventQueue.Enqueue(newEvent);
                    }
                }
            }
            
            iterations++;
        }
        
        if (iterations >= _maxIterations)
        {
            return EventProcessingResult.Error("Max event iterations reached - possible infinite loop");
        }
        
        return EventProcessingResult.Success(allProcessedEvents);
    }
}

public record EventProcessingResult(
    bool IsSuccess,
    string? ErrorMessage,
    IReadOnlyList<IGameEvent> ProcessedEvents
)
{
    public static EventProcessingResult Success(IReadOnlyList<IGameEvent> events) 
        => new(true, null, events);
    
    public static EventProcessingResult Error(string message) 
        => new(false, message, Array.Empty<IGameEvent>());
}
```

## Integration with Actions

### Event-Aware Node Base Class

```csharp
public abstract class EventAwareNode : LinkableNode
{
    protected EventQueue EventQueue { get; private set; } = new();
    protected EventProcessor EventProcessor { get; private set; }
    
    protected EventAwareNode()
    {
        EventProcessor = new EventProcessor(EventQueue);
    }
    
    protected void EmitEvent(IGameEvent gameEvent)
    {
        EventQueue.Enqueue(gameEvent);
    }
    
    protected EventProcessingResult ProcessPendingEvents(GameStateView state)
    {
        return EventProcessor.ProcessEvents(state);
    }
}
```

### Example: Move Action with Events

```csharp
public class MoveActionNode : EventAwareNode
{
    public override NodeId Id => new("Move_Execute");
    
    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        var ctx = GetTypedContext<MoveActionContext>(context);
        
        if (!ctx.TryGet<EntityId>("EntityId", out var entityId)) 
            return ActionStepResult.Fail("EntityId missing");
        
        if (!ctx.TryGet<IBoardPosition>("Destination", out var dest)) 
            return ActionStepResult.Fail("Destination missing");
        
        var currentPos = state.GetPosition(entityId);
        
        // 1. Execute the move
        state.RecordOperation(new MoveOperation(entityId, dest));
        
        // 2. Emit movement event
        EmitEvent(new EntityMovedEvent(
            EventId.New(),
            entityId,
            currentPos!,
            dest
        ));
        
        // 3. Process all cascading events
        var result = ProcessPendingEvents(state);
        
        if (!result.IsSuccess)
        {
            return ActionStepResult.Fail(result.ErrorMessage!);
        }
        
        // 4. Store events for UI/Animation
        ctx.Set("ProcessedEvents", result.ProcessedEvents);
        
        return ActionStepResult.Success();
    }
}
```

## Use Cases

### 1. Parchis Capture Mechanic

```csharp
public class CaptureReaction : IEventReaction
{
    public bool ShouldReact(IGameEvent gameEvent, GameStateView state)
    {
        if (gameEvent is not EntityMovedEvent moved) return false;
        
        // Check if there's an enemy pawn at destination
        var pawnsAtDest = state.GetEntitiesAt(moved.To).OfType<Actor>();
        var movingPawn = state.GetEntity(moved.EntityId) as Actor;
        
        return pawnsAtDest.Any(p => p.Team != movingPawn?.Team);
    }
    
    public IEnumerable<IGameEvent> Execute(IGameEvent triggerEvent, GameStateView state)
    {
        var moved = (EntityMovedEvent)triggerEvent;
        var movingPawn = state.GetEntity(moved.EntityId) as Actor;
        
        var enemyPawns = state.GetEntitiesAt(moved.To)
            .OfType<Actor>()
            .Where(p => p.Team != movingPawn?.Team);
        
        foreach (var enemy in enemyPawns)
        {
            // Send enemy back to spawn
            var spawnPos = new TilePosition(new TileId($"spawn_{enemy.Team}"));
            state.RecordOperation(new MoveOperation(enemy.Id, spawnPos));
            
            yield return new EntityCapturedEvent(
                EventId.New(),
                enemy.Id,
                moved.EntityId,
                spawnPos
            );
        }
    }
}
```

### 2. Trap Activation

```csharp
public class TrapActivationReaction : IEventReaction
{
    private readonly EntityId _trapId;
    private readonly int _damage;
    
    public TrapActivationReaction(EntityId trapId, int damage)
    {
        _trapId = trapId;
        _damage = damage;
    }
    
    public bool ShouldReact(IGameEvent gameEvent, GameStateView state)
    {
        if (gameEvent is not EntityMovedEvent moved) return false;
        
        var trapPos = state.GetPosition(_trapId);
        return moved.To.Equals(trapPos);
    }
    
    public IEnumerable<IGameEvent> Execute(IGameEvent triggerEvent, GameStateView state)
    {
        var moved = (EntityMovedEvent)triggerEvent;
        
        // Apply damage
        state.RecordOperation(new DamageOperation(moved.EntityId, _damage));
        
        yield return new EntityDamagedEvent(
            EventId.New(),
            moved.EntityId,
            _trapId,
            _damage
        );
        
        // Destroy trap
        state.RecordOperation(new DestroyOperation(_trapId));
        
        yield return new EntityDestroyedEvent(
            EventId.New(),
            _trapId,
            null
        );
    }
}
```

### 3. Death Triggers

```csharp
public class DeathExplosionReaction : IEventReaction
{
    private readonly EntityId _entityId;
    private readonly int _explosionRadius;
    private readonly int _explosionDamage;
    
    public DeathExplosionReaction(EntityId entityId, int radius, int damage)
    {
        _entityId = entityId;
        _explosionRadius = radius;
        _explosionDamage = damage;
    }
    
    public bool ShouldReact(IGameEvent gameEvent, GameStateView state)
    {
        return gameEvent is EntityDestroyedEvent destroyed 
            && destroyed.EntityId == _entityId;
    }
    
    public IEnumerable<IGameEvent> Execute(IGameEvent triggerEvent, GameStateView state)
    {
        var destroyed = (EntityDestroyedEvent)triggerEvent;
        var explosionCenter = state.GetPosition(_entityId);
        
        if (explosionCenter == null) yield break;
        
        // Find all entities within radius
        var nearbyEntities = FindEntitiesInRadius(state, explosionCenter, _explosionRadius);
        
        foreach (var entity in nearbyEntities)
        {
            state.RecordOperation(new DamageOperation(entity.Id, _explosionDamage));
            
            yield return new EntityDamagedEvent(
                EventId.New(),
                entity.Id,
                _entityId,
                _explosionDamage
            );
        }
    }
    
    private IEnumerable<GameEntity> FindEntitiesInRadius(
        GameStateView state, 
        IBoardPosition center, 
        int radius)
    {
        // Implementation depends on board topology
        // Could use board.GetEntitiesInRange() or similar
        yield break; // Placeholder
    }
}
```

## Execution Flow

```
User Action (e.g., Move)
    ↓
1. Execute Operation (MoveOperation)
    ↓
2. Emit Event (EntityMovedEvent)
    ↓
3. Enqueue Event
    ↓
4. Process Event Queue
    ↓
    While queue has events:
        ↓
        Dequeue Event
        ↓
        For each registered Reaction:
            ↓
            If ShouldReact(event):
                ↓
                Execute Reaction
                ↓
                Enqueue new events (if any)
    ↓
5. Return all processed events
    ↓
6. Store events in context for UI/Animation
    ↓
Action Complete
```

## Benefits

1. **✅ Controlled Execution**: Sequential processing with iteration limits prevents infinite loops
2. **✅ Cascading Events**: Events can naturally trigger more events
3. **✅ Separation of Concerns**: Reactions are separate from entities and actions
4. **✅ Testable**: Each reaction can be tested in isolation
5. **✅ Transactional**: All changes occur in the overlay, can be reverted
6. **✅ UI-Friendly**: Event history available for animations and visual feedback
7. **✅ Extensible**: Easy to add new event types and reactions
8. **✅ Debuggable**: Full event log for debugging game logic

## Implementation Considerations

### Performance
- Event processing is O(n*m) where n = events, m = reactions
- Consider indexing reactions by event type for large numbers of reactions
- Use `yield return` to avoid allocating lists unnecessarily

### Safety
- Max iteration limit prevents infinite event loops
- Consider adding cycle detection for more sophisticated loop prevention
- Validate that reactions don't create contradictory state changes

### Testing
- Unit test each reaction independently
- Integration test event chains
- Test max iteration limit behavior

### UI Integration
- Events can be consumed by UI layer for animations
- Event timestamps enable replay functionality
- Consider adding event serialization for game replays

## Future Enhancements

1. **Event Priorities**: Some reactions should execute before others
2. **Conditional Reactions**: Reactions that only trigger under certain conditions
3. **Event Filtering**: Ability to filter events by type, source, etc.
4. **Event Replay**: Store and replay event sequences for debugging
5. **Network Sync**: Serialize events for multiplayer synchronization
6. **Component-Based Reactions**: Attach reactions to entity components

## Related Systems

- **Action System**: Actions emit events during execution
- **Overlay System**: All reactions modify state via overlay operations
- **Component System**: Components can define their own reactions
- **FSM System**: State transitions can be triggered by events

## Status

**Status**: Proposed Architecture (Not Implemented)
**Date**: 2026-01-04
**Author**: System Design Discussion
