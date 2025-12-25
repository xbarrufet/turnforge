# Common Patterns & Extensions

## Fluent Builders

We recommend using Fluent Builders for creating complex objects like SpawnCommands.

```csharp
// TurnForge pattern
var request = SpawnRequestBuilder
    .For("DefinitionId")
    .At(position)
    .Build();
```

## Property Auto-Mapping

The engine automatically maps properties between Definitions, Descriptors, and Component interfaces if they share the same **name** and **type**.

To override this, use attributes:

```csharp
[MapToComponent(typeof(IStatsComponent), "Strength")]
public int Str { get; set; }
```

## Custom Attributes System

Use the `AttributeComponent` for dynamic data that doesn't fit into a strongly-typed component.

See [Dynamic Attributes](attributes.md) for full details.
