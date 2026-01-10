# TurnForge.Engine.Tests

## Customizing Entities via Definitions and Descriptors

This test suite demonstrates how to customize base entities (Agent, Prop, Zone) **without subclassing** by providing:
- Custom **traits** in Definitions
- Optional **trait overrides** and **extra components** in Descriptors

### How automatic component materialization works
- `TraitInitializationService` scans traits on the entity and materializes matching runtime components using a convention:
  - A component type must implement `IGameEntityComponent`
  - Be public and non-abstract
  - Provide a public constructor with exactly one parameter of the trait type
- Example: `CustomSpeedTrait` → `CustomSpeedComponent(CustomSpeedTrait trait)`

### Files
- `Entities/TestClasses.cs`
  - Test definitions and descriptors for Agent/Prop/Zone
  - `CustomSpeedTrait` and `CustomSpeedComponent` to showcase trait→component wiring
  - `CustomTestAgentDefinition` uses the custom trait
  - `CustomTestAgentDescriptor` optionally allows trait overrides / extra components
- `Entities/Factory/CreateBasEntitiesTest.cs`
  - Baseline tests for Agent/Prop/Zone creation
- `Entities/Factory/CreateCustomEntitiesTest.cs`
  - Validates that a custom agent definition with a custom trait produces a runtime component automatically via `TraitInitializationService`

### Running tests

```bash
# Run all engine tests
 dotnet test tests/TurnForge.Engine.Tests/TurnForge.Engine.Tests.csproj

# Run only custom entity tests
 dotnet test tests/TurnForge.Engine.Tests/TurnForge.Engine.Tests.csproj --filter "FullyQualifiedName~CreateCustomEntitiesTest"
```

### Notes
- No user-defined GameEntity subclasses are required. All customization is data-driven via traits/components on Definitions/Descriptors.
- If your component does not appear, check constructor signature, visibility, and that the trait is present on the entity.

