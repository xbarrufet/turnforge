# Entities & Spawning

TurnForge uses a Definition-Descriptor-Entity pattern to provide both flexibility and type safety.

## 1. Defining Custom Entities

To create a new game entity (e.g., a "Survivor"), follow these steps:

### Step 1: Create the Entity Class
Inherit from `Agent` (active) or `Prop` (passive). Decorate with `[DefinitionType]` and options `[DescriptorType]`.

```csharp
[DefinitionType(typeof(SurvivorDefinition))]
public class Survivor : Agent
{
    public Survivor(EntityId id, string definitionId, string name, string category)
        : base(id, definitionId, name, category)
    {
        // Add default components
        AddComponent(new FactionComponent());
        AddComponent(new InventoryComponent());
    }
}
```

### Step 2: Create the Definition
Inherit from `AgentDefinition`. Properties matching component names map automatically.

```csharp
[EntityType(typeof(Survivor))]
public class SurvivorDefinition : AgentDefinition
{
    // Maps to IHealthComponent.MaxHealth automatically
    public int MaxHealth { get; set; } = 10;
    
    // Explicit mapping to specific component property
    [MapToComponent(typeof(IFactionComponent), "FactionName")]
    public string Faction { get; set; } = "Survivors";
}
```

### Step 3: Register in Catalog
Add your definitions to the `IGameCatalog`.

```csharp
catalog.AddDefinition(new SurvivorDefinition("Survivors.Mike", "Mike", "Survivor") 
{ 
    MaxHealth = 12,
    Faction = "Police"
});
```

## 2. Spawning Entities

Use `SpawnRequestBuilder` to spawn entities during gameplay.

```csharp
var spawnRequest = SpawnRequestBuilder
    .For("Survivors.Mike")
    .At(new Position(10, 5))
    .WithProperty("MaxHealth", 15) // Override definition value
    .Build();

var command = new SpawnAgentsCommand(new[] { spawnRequest });
engine.ExecuteCommand(command);
```

## 3. Custom Descriptors (Optional)

For complex spawn logic, create a custom `Descriptor`.

```csharp
public class SurvivorDescriptor : AgentDescriptor
{
    public string StartingWeapon { get; set; }
    
    // Required constructor
    public SurvivorDescriptor(string definitionId) : base(definitionId) { }
}

[DescriptorType(typeof(SurvivorDescriptor))]
public class Survivor : Agent { ... }
```
