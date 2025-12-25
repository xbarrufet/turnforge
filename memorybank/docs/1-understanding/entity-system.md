# Entity System

## Entity Hierarchy

```
GameEntity (abstract)
├── Actor (has position, health)
│   ├── Agent (can take actions)
│   └── Prop (static environmental object)
└── Item (pickupable, no position) [NOT IMPLEMENTED]
```

## Components

Components are atomic pieces of entity data. All entity mutations operate at component level.

**Base Components:**
- `BehaviourComponent`: Dynamic behaviors (e.g., "Fly", "Swim")
- `PositionComponent`: Tile position on board
- `HealthComponent`: Current/max health
- `MovementComponent`: Movement points (Agents only)
- `ActionTakeComponent`: Action points (Agents only)

**Component Example:**
```csharp
public interface IHealthComponent : IGameEntityComponent
{
    int MaxHealth { get; set; }
    int CurrentHealth { get; set; }
}
```

## Definition-Descriptor Pattern (Auto-Mapping)

**Definition** = Blueprint (Data Source)
```csharp
[EntityType(typeof(Survivor))]
public class SurvivorDefinition : AgentDefinition
{
    // FT-004: Implicit Mapping
    // Matches properties in IHealthComponent by name/type automatically.
    public int MaxHealth { get; set; } = 10;
    
    // Explicit mapping for different names
    [MapToComponent(typeof(IFactionComponent), "FactionName")]
    public string Team { get; set; } = "Survivors";
}
```

**Descriptor** = Spawn Request (Overrides)
```csharp
public class SurvivorDescriptor : AgentDescriptor
{
    public SurvivorDescriptor(string definitionId) : base(definitionId) { }
    
    // Automatically maps to IPositionComponent.CurrentPosition if matched
    // Or used by Strategy/Factory logic
    public Position Position { get; set; }
}
```

**Property Discovery (FT-004):**
1. **Implicit (Default):** Properties on Definition/Descriptor map to Component properties with same name and type.
2. **Explicit (`[MapToComponent]`):** Redirects mapping to specific specific component/property.
3. **Opt-Out (`[DoNotMap]`):** Attributes on Component properties prevent external setting.

**Factory Process:**
1. Handler creates `Descriptor` from command
2. `SpawnStrategy` validates and builds `SpawnDecision<Descriptor>`
3. `SpawnApplier` fetches `Definition`, creates entity, maps properties
4. Result: Entity in `GameState` with merged Definition + Descriptor data

## Component Lookup

Supports lookup by interface or concrete type:

```csharp
entity.AddComponent(new FactionComponent()); // Register as concrete

// Lookup by interface (auto-resolved)
var faction = entity.GetComponent<IFactionComponent>(); // ✅ Works
```

**Implementation:** Fallback search if direct key not found.
