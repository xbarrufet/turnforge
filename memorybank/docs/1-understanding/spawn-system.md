# Spawn System

The Spawn Command system handles entity creation with a flexible, type-safe pipeline.

**Pipeline Flow:**
```
SpawnRequest → DescriptorBuilder → Descriptor → Strategy → Decision → Applier → Entity
```

**Key Principle:** Entities are **ONLY** created via Spawn. Updates are done via Components.

## SpawnRequest (User Input)

The `SpawnRequest` is the user-facing API for requesting entity spawns.

```csharp
public sealed record SpawnRequest(
    string DefinitionId,         // Required
    int Count = 1,               // Optional: batch spawn
    IEnumerable<IBaseTrait>? TraitsToOverride = null // Optional: Override characteristics
);
```

**Usage (Fluent Builder - Recommended):**
```csharp
var request = SpawnRequestBuilder
    .For("Survivors.Mike")
    .At(spawnPoint)
    .WithTrait(new ActionPointsTrait(12))
    .Build();
```

## Architecture

| Component | Responsibility |
|-----------|----------------|
| **SpawnRequestBuilder** | Fluent API for creating requests |
| **DescriptorBuilder** | Converts generic Request to typed Descriptor (using reflection) |
| **Descriptor** | Typed DTO containing all valid properties for the entity |
| **ISpawnStrategy** | Business logic: validating spawn, finding positions |
| **ISpawnApplier** | Creation logic: invoking factory, updating GameState |
| **Factory** | Instantiating entity and mapping properties |

## Hierarchical Strategy System

For game-specific spawn logic, TurnForge provides a hierarchical strategy system with compile-time type safety via the `EntityTypeRegistry`.

### Entity Type Registry

**Purpose:** Runtime type lookup for Definition→Entity→Descriptor relationships.

```csharp
// Initialization
EntityTypeRegistry.Initialize();
```

**Lookup Methods:**
```csharp
var entityType = EntityTypeRegistry.GetEntityType(typeof(SurvivorDefinition)); // → Survivor
var descriptorType = EntityTypeRegistry.GetDescriptorType(typeof(Survivor)); // → SurvivorDescriptor
```
