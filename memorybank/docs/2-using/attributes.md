# Dynamic Attribute System

The **Dynamic Attribute System** allows you to define arbitrary statistics for your game entities without modifying the engine code. Attributes can be simple integer values (like Strength, ActionPoints) or dice-based formulas (like Damage, LootQuality).

## 1. Defining Attributes

Attributes are defined in the `Attributes` section of your Entity Definition JSON file.

### Example: Character Definition
```json
{
  "DefinitionId": "Survivor.Mike",
  "Name": "Mike",
  "Category": "Survivor",
  "Attributes": {
    "Strength": 5,
    "Agility": 3,
    "MeleeDamage": "1D6+2",
    "CritChance": "1D20"
  },
  "Components": { ... }
}
```

The system automatically detects the type:
- **Integer (5)**: Stored as a fixed value.
- **String ("1D6+2")**: Parsed as a dice formula.

## 2. Accessing Attributes in Code

Attributes are stored in the `AttributeComponent`. You can retrieve them by name.

```csharp
var attributes = entity.GetComponent<AttributeComponent>();

// 1. Get a simple value
if (attributes.Get("Strength") is AttributeValue strength) 
{
    Console.WriteLine($"Strength: {strength.CurrentValue}");
}

// 2. checking for a dice attribute
if (attributes.Get("MeleeDamage") is AttributeValue damage)
{
    if (damage.IsDiceAttribute)
    {
        Console.WriteLine($"Damage Roll: {damage.Dice}"); // Prints "1D6+2"
        
        // Rolling logic (requires DiceService implementation)
        // var roll = DiceService.Roll(damage.Dice);
    }
}
```

## 3. Dice Notation

The system supports standard RPG dice notation:

| Notation | Meaning |
|----------|---------|
| `1D6` | Roll one 6-sided die |
| `2D6` | Roll two 6-sided dice |
| `1D20+5` | Roll one 20-sided die and add 5 |
| `3D8-1` | Roll three 8-sided dice and subtract 1 |

## 4. Updates & Modifiers

The `AttributeComponent` is immutable. To modify an attribute (e.g., taking damage or receiving a buff), you must replace the component.

*Note: A dedicated `AttributeUpdateApplier` will be added in future updates to simplify this process using commands.*

```csharp
// Manual update example
var current = attributes.Get("Strength").Value;
var newStrength = current.WithCurrent(current.CurrentValue + 1); // +1 Buff
var newComponent = attributes.Set("Strength", newStrength);

// Apply change to entity
entity.ReplaceComponent(newComponent);
```
