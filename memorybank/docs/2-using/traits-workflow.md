# Traits & Components Workflow

TurnForge uses a **Trait-based architecture** to define entities. This separates **Data (Traits)** from **Logic/State (Components)**, ensuring a clean and extensible system.

## Core Concepts

*   **Trait (Source of Truth)**: A lightweight, serializable data object (Record or Class) that defines *what* an entity is or has. It contains initial configuration data.
*   **Component (Runtime State)**: A logic-heavy object that defines *how* an entity behaves. It holds mutable runtime state.
*   **Initialization Service**: Automatically wires Traits to Components during entity creation.

---

## 🚀 Workflow: Creating a New Ability/Feature

### 1. Define the Trait (Data)

Create a class inheriting from `BaseTrait` (or `IBaseTrait`). This class should hold **configuration data** only.

```csharp
using TurnForge.Engine.Traits.Standard;

public class ActionPointsTrait : BaseTrait
{
    public int MaxActionPoints { get; private set; }
    public int RegenerationRate { get; private set; }

    public ActionPointsTrait() { } // Required for serialization

    public ActionPointsTrait(int maxActionPoints, int regenerationRate = 1)
    {
        MaxActionPoints = maxActionPoints;
        RegenerationRate = regenerationRate;
    }
}
```

### 2. Define the Component (Logic)

Create a component implementing `IGameEntityComponent` (or extending trait-specific interfaces).

**Crucial Step**: Add a constructor that accepts your `Trait`. This is how the engine knows how to initialize the component.

```csharp
using TurnForge.Engine.Components.Interfaces;

public class BaseActionPointsComponent : IActionPointsComponent
{
    public int MaxActionPoints { get; private set; }
    public int CurrentActionPoints { get; set; }

    // 🌟 MAGIC HAPPENS HERE 🌟
    // The TraitInitializationService looks for this specific constructor signature.
    public BaseActionPointsComponent(ActionPointsTrait trait)
    {
        MaxActionPoints = trait.MaxActionPoints;
        CurrentActionPoints = trait.MaxActionPoints; // Initialize state
    }

    public void Regenerate() 
    {
        // Logic...
    }
}
```

### 3. Usage

#### Automatic Wiring
When a definition or spawn request includes `ActionPointsTrait`, the engine will automatically:
1.  Detect `ActionPointsTrait` in the list of requested traits.
2.  Find the Component (`BaseActionPointsComponent`) that has a constructor accepting `ActionPointsTrait`.
3.  Instantiate the component using that constructor.
4.  Add the component to the entity.

#### Spawning with Traits

Use the fluent builder to override or add traits at spawn time:

```csharp
var request = SpawnRequestBuilder
    .For("Survivors.Mike")
    .At(spawnPoint)
    .WithTrait(new ActionPointsTrait(5, 2)) // Override default AP
    .WithTrait(new TeamTrait("Rebels"))     // Set Team
    .Build();

engine.Execute(new SpawnAgentsCommand(new [] { request }));
```

---

## Architecture Benefits

1.  **Separation of Concerns**: Configuration is decoupled from runtime logic.
2.  **Serialization**: Traits are easy to serialize (JSON) for saving/loading templates.
3.  **Flexibility**: You can attach any trait to any entity without changing the class hierarchy.
4.  **Type Safety**: Traits are strongly typed, avoiding "magic string" property dictionaries.
