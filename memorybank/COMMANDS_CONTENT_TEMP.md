# COMMANDS Section Content (to be inserted in ENTIDADES.md)

## COMMANDS

Commands are the primary way to interact with the TurnForge engine. They represent user or AI intentions and are validated, processed, and executed through the Command-Decision-Applier pipeline.

### Command Structure

```csharp
public interface ICommand 
{ 
    Type CommandType { get; } 
}
```

All commands must:
- Be immutable (use `record` types)
- Contain all necessary data
- Not perform business logic (that's in handlers)

---

### Spawn Command

The Spawn Command system handles entity creation with a flexible, type-safe pipeline.

#### Architecture Overview

```mermaid
graph TD
    A[User Code] -->|creates| B[SpawnRequest]
    A -->|or uses| B2[SpawnRequestBuilder]
    B2 -->|Build| B
    B -->|SpawnCommand| C[CommandHandler]
    C -->|preprocessor| D[DescriptorBuilder]
    D -->|creates| E[AgentDescriptor/PropDescriptor]
    E[ISpawnStrategy| F[Process/Validate]
    F -->|ToDecisions| G[SpawnDecision<T>]
    G -->|Orchestrator| H[SpawnApplier]
    H -->|Factory| I[Entity Created]
    
    style B fill:#87CEEB
    style B2 fill:#90EE90
    style D fill:#FFE4B5
    style E fill:#FFE4B5
```

**Pipeline Flow:**
```
SpawnRequest → Descriptor → Strategy → Decision → Applier → Entity
```

**Key Principle:** Entities are **ONLY** created via Spawn. Updates are done via Components.

---

#### SpawnRequest (User Input)

The `SpawnRequest` is the user-facing API for requesting entity spawns.

**Structure:**
```csharp
public sealed record SpawnRequest(
    string DefinitionId,  // Required: entity definition ID
    int Count = 1,        // Optional: batch spawn
    Position? Position = null,  // Optional: strategy decides if null
    Dictionary<string, object>? PropertyOverrides = null,  // Optional: override properties
    IEnumerable<IGameEntityComponent>? ExtraComponents = null // Optional: extra components
);
```

**Usage (Direct):**
```csharp
var request = new SpawnRequest(
    "Survivors.Mike",
    Position: spawnPoint,
    PropertyOverrides: new() { 
        ["Health"] = 12,
        ["Faction"] = "Police" 
    }
);
```

**Usage (Fluent Builder - Recommended):**
```csharp
var request = SpawnRequestBuilder
    .For("Survivors.Mike")
    .At(spawnPoint)
    .WithProperty("Health", 12)
    .WithProperty("Faction", "Police")
    .Build();
```

---

#### SpawnRequestBuilder (Fluent API)

**Purpose:** Improve developer experience with IntelliSense-driven API while producing standard `SpawnRequest` objects.

**File:** [`TurnForge.Engine/Commands/Spawn/SpawnRequestBuilder.cs`](file:///Users/barrufex/Development/TurnForge/src/TurnForge.Engine/Commands/Spawn/SpawnRequestBuilder.cs)

**Features:**
- ✅ Fluent method chaining
- ✅ Type-safe generic overloads
- ✅ Implicit conversion to `SpawnRequest`
- ✅ Validation on build

**Example - Simple Spawn:**
```csharp
// Before (verbose for complex cases)
var boss = new SpawnRequest(
    "Enemies.DragonBoss",
    Count: 1,
    Position: bossSpawn,
    PropertyOverrides: new Dictionary<string, object>
    {
        ["Health"] = 1000,
        ["PhaseCount"] = 5,
        ["Faction"] = "Undead"
    },
    ExtraComponents: new[] { component1, component2 }
);

// After (fluent, discoverable)
var boss = SpawnRequestBuilder
    .For("Enemies.DragonBoss")
    .At(bossSpawn)
    .WithProperty("Health", 1000)
    .WithProperty("PhaseCount", 5)
    .WithProperty("Faction", "Undead")
    .WithComponents(component1, component2)
    .Build();
```

**Example - Batch Spawn with LINQ:**
```csharp
var zombies = Enumerable.Range(0, 10)
    .Select(i => SpawnRequestBuilder
        .For("Enemies.Zombie")
        .At(spawnPoints[i])
        .WithProperty("Health", 10 + (i * 5))
        .WithProperty("Speed", i % 2 == 0 ? "Fast" : "Slow")
        .Build())
    .ToList();
```

**Available Methods:**
- `For(string definitionId)` - Entry point (static factory)
- `At(Position position)` - Set spawn position
- `WithCount(int count)` - Set batch count
- `WithProperty(string key, object value)` - Add property override
- `WithProperty<T>(string key, T value)` - Type-safe property override
- `WithComponent<T>(T component)` - Add extra component
- `WithComponents(params IGameEntityComponent[])` - Add multiple components
- `Build()` - Create final `SpawnRequest`

**Implicit Conversion:**
```csharp
// Can omit .Build() in assignments
SpawnRequest request = SpawnRequestBuilder
    .For("Survivors.Mike")
    .At(spawnPoint);

// Works in command constructors
var cmd = new SpawnAgentsCommand(new[]
{
    SpawnRequestBuilder.For("Survivors.Mike").At(spawn1).Build(),
    SpawnRequestBuilder.For("Survivors.Doug").At(spawn2).Build()
});
```

#### DescriptorBuilder (Preprocessor)

**Purpose:** Convert `SpawnRequest` to typed descriptors automatically.

**File:** `TurnForge.Engine/Core/Factories/DescriptorBuilder.cs`

**Process:**
1. Create descriptor instance (respects `[DescriptorType]` attribute)
2. Apply property overrides via reflection
3. Copy position if specified
4. Copy extra components

**Code:**
```csharp
public static TDescriptor Build<TDescriptor>(
    SpawnRequest request,
    BaseGameEntityDefinition definition)
    where TDescriptor : IGameEntityBuildDescriptor
{
    // 1. Create descriptor (may use custom type via attribute)
    var descriptor = CreateDescriptor<TDescriptor>(request.DefinitionId, definition);
    
    // 2. Map Dictionary → Properties via reflection
    if (request.PropertyOverrides != null)
    {
        ApplyOverrides(descriptor, request.PropertyOverrides);
    }
    
    // 3. Set position
    if (request.Position.HasValue)
    {
        descriptor.Position = request.Position.Value;
    }

    // 4. Copy extra components
    if (request.ExtraComponents != null)
    {
        descriptor.ExtraComponents.AddRange(request.ExtraComponents);
    }
    
    return descriptor;
}
```

**Custom Descriptor Support:**
```csharp
// Definition can specify custom descriptor
[DescriptorType(typeof(BossAgentDescriptor))]
public class BossDefinition : AgentDefinition
{
    public int PhaseCount { get; init; }
}

// DescriptorBuilder automatically creates BossAgentDescriptor
// User still uses standard SpawnRequest with PropertyOverrides
```

---

#### Descriptors (Internal Type Safety)

Descriptors are **internal transformation artifacts** that provide type safety during spawn processing.

**Base Descriptor:**
```csharp
public class GameEntityBuildDescriptor : IGameEntityBuildDescriptor
{
    public string DefinitionID { get; set; }
    public List<IGameEntityComponent> ExtraComponents { get; init; } = new();
    public Position? Position { get; set; }
}
```

**Specialized Descriptors:**
```csharp
public class AgentDescriptor : GameEntityBuildDescriptor { }
public class PropDescriptor : GameEntityBuildDescriptor { }
```

**Custom Descriptors (Optional):**
```csharp
public class BossAgentDescriptor : AgentDescriptor
{
    public int PhaseCount { get; set; }
    public List<string> MinionPacks { get; set; } = new();
}
```

**Key Point:** Users **do not** define custom descriptors. The engine uses them internally for type-safe processing.

---

#### Spawn Strategy

**Purpose:** Process descriptors and determine spawn conditions (position, validation, filtering).

**Interface:**
```csharp
public interface ISpawnStrategy<TDescriptor> 
    where TDescriptor : IGameEntityBuildDescriptor
{
    // Process descriptors (filter, modify, validate)
    IReadOnlyList<TDescriptor> Process(
        IReadOnlyList<TDescriptor> descriptors,
        GameState state)
    {
        return descriptors; // Default: accept all
    }
    
    // Convert to decisions (usually doesn't need override)
    IReadOnlyList<SpawnDecision<TDescriptor>> ToDecisions(
        IReadOnlyList<TDescriptor> descriptors)
    {
        return descriptors
            .Select(d => new SpawnDecision<TDescriptor>(d))
            .ToList();
    }
}
```

**Responsibilities:**
- ✅ Assign spawn positions (if not provided)
- ✅ Validate spawn conditions
- ✅ Filter invalid spawns
- ✅ Modify descriptor properties based on game state
- ❌ NOT create entities (that's the applier's job)

**Example - Survivor Spawn Strategy:**
```csharp
public class ConfigurableAgentSpawnStrategy : ISpawnStrategy<AgentDescriptor>
{
    public IReadOnlyList<AgentDescriptor> Process(
        IReadOnlyList<AgentDescriptor> descriptors,
        GameState state)
    {
        foreach (var descriptor in descriptors)
        {
            // Determine spawn position based on definition
            var spawnPosition = FindSpawnPoint(descriptor.DefinitionID, state);
            
            if (spawnPosition == Position.Empty)
            {
                _logger?.LogWarning($"No spawn position found for {descriptor.DefinitionID}");
            }
            
            // Update descriptor with determined position
            descriptor.Position = spawnPosition;
        }
        
        return descriptors;
    }
    
    private Position FindSpawnPoint(string definitionId, GameState state)
    {
        // Find spawn prop based on entity category
        if (definitionId.Contains("Survivor"))
        {
            var spawnPoint = state.GetProps()
                .FirstOrDefault(p => p.DefinitionId == "Spawn.Player");
            return spawnPoint?.PositionComponent?.CurrentPosition ?? Position.Empty;
        }
        
        // ... other entity types
        
        return Position.Empty;
    }
}
```

**Strategy Registration:**
```csharp
// Register in DI container
services.RegisterSingleton<ISpawnStrategy<AgentDescriptor>>(
    new ConfigurableAgentSpawnStrategy(logger));

services.RegisterSingleton<ISpawnStrategy<PropDescriptor>>(
    new ConfigurablePropSpawnStrategy(logger));
```

---

To be continued in next part...
