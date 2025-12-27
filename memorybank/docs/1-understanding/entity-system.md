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
- `TraitContainerComponent`: Dynamic traits (e.g., "Fly", "Swim")
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

## Trait-Based Architecture

Entities are defined by **Traits**.

*   **Definition**: A collection of default Traits.
*   **Descriptor**: A collection of override Traits (requested at spawn).
*   **Initialization**: The `TraitInitializationService` converts Traits into mutable **Components**.

**Process:**
1.  **Definition**: "Mike" has `VitalityTrait(10)` and `TeamTrait("Survivors")`.
2.  **Spawn Request**: requesting "Mike" but overrides with `VitalityTrait(5)`.
3.  **Merge**: Final Trait set = `VitalityTrait(5)`, `TeamTrait("Survivors")`.
4.  **Hydration**:
    *   `VitalityTrait` -> finds component with `ctor(VitalityTrait)` -> Creates `HealthComponent(5)`.
    *   `TeamTrait` -> finds component with `ctor(TeamTrait)` -> Creates `TeamComponent("Survivors")`.

This eliminates the need for complex reflection-based property mapping ("Auto-Mapping").

## Component Lookup

Supports lookup by interface or concrete type:

```csharp
entity.AddComponent(new FactionComponent()); // Register as concrete

// Lookup by interface (auto-resolved)
var faction = entity.GetComponent<IFactionComponent>(); // ✅ Works
```

**Implementation:** Fallback search if direct key not found.
