# Components & Traits

** OBSOLETE TO BE REVIEWED **

**Traits** define the identity of an entity. **Components** show the entity current state.
All state mutation happens by replacing components on immutable entities.

## Standard Components

TurnForge provides these base components:

| Component | Interface | Purpose |
|-----------|-----------|---------|
| **Position** | `IPositionComponent` | Tracks board location |
| **Health** | `IHealthComponent` | Max/Current health |
| **Action Points** | `IActionPointsComponent` | Turn resource management |
| **Movement** | `IMovementComponent` | Movement range/capability |
| **Attribute** | `IAttributeComponent` | Dynamic stats (Str, Dex, etc.) |

## Creating Custom Components

### 1. Define Interface
Inherit from `IGameEntityComponent`.

```csharp
public interface IInventoryComponent : IGameEntityComponent
{
    IReadOnlyList<string> ItemIds { get; }
    int Capacity { get; }
}
```

### 2. Implement Component
Ideally make it immutable.

```csharp
public record InventoryComponent(
    IReadOnlyList<string> ItemIds,
    int Capacity
) : IInventoryComponent
{
    // Default constructor
    public InventoryComponent() : this(ImmutableList<string>.Empty, 10) { }

    // Helper for mutation
    public InventoryComponent AddItem(string itemId)
    {
        return this with { ItemIds = ((ImmutableList<string>)ItemIds).Add(itemId) };
    }
}
```

### 3. Using Components

```csharp
// Access
if (agent.GetComponent<IInventoryComponent>() is InventoryComponent inventory)
{
    // Mutate (creates new component)
    var newInventory = inventory.AddItem("Sword");
    
    // Update Entity (creates new entity)
    var updatedAgent = agent.ReplaceComponent(newInventory);
}
```
