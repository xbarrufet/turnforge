# Configuration Attributes Reference

These C# attributes are used to configure the Entity System and Property Auto-Mapping.

## Entity Configuration

| Attribute | Target | Purpose |
|-----------|--------|---------|
| `[DefinitionType(Type)]` | Entity Class | Links an Entity class to its Definition type. |
| `[DescriptorType(Type)]` | Entity Class | Links an Entity class to its Descriptor type. |
| `[EntityType(Type)]` | Definition Class | Links a Definition back to the Entity class it creates. |

**Example:**
```csharp
[DefinitionType(typeof(SurvivorDefinition))]
[DescriptorType(typeof(SurvivorDescriptor))]
public class Survivor : Agent { ... }
```

## Mapping Configuration

Used by `PropertyAutoMapper`.

| Attribute | Target | Purpose |
|-----------|--------|---------|
| `[MapToComponent(Type, Name)]` | Definition/Descriptor Property | Redirects property mapping to a specific component/property name. |
| `[DoNotMap]` | Component Property | Prevents external mapping to this property. |

**Example:**
```csharp
public class SurvivorDefinition
{
    // Maps to IFactionComponent.FactionName instead of property name "Team"
    [MapToComponent(typeof(IFactionComponent), "FactionName")]
    public string Team { get; set; }
}
```
