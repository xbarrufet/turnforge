# Test Scenario Infrastructure

Fluent test infrastructure for creating E2E scenarios programmatically in BarelyAlive tests.

## Overview

This infrastructure provides three main components:

- **`ScenarioRunner`** - Fluent Given/When/Then API for E2E scenarios
- **`CommandBuilder`** - Fluent command creation helpers
- **`ScenarioSerializer`** - JSON scenario loading/saving

## Quick Start

###Basic Scenario

```csharp
using BarelyAlive.Rules.Tests.Helpers;

ScenarioRunner.Create()
    .GivenMission(TestHelpers.Mission01Json)
    .GivenSurvivors("Survivor.Mike", "Survivor.Doug")
    .Then(state =>
    {
        Assert.That(state.GetAgents().Count, Is.EqualTo(2));
    });
```

### Move Command

```csharp
var targetTile = new TileId(Guid.Parse("66a0dadc-d774-4ce9-a3ec-0213e9528af6"));

ScenarioRunner.Create()
    .GivenMission(TestHelpers.Mission01Json)
    .GivenSurvivors("Survivor.Mike")
    .When(cmd => cmd.Move("Survivor.Mike").To(targetTile))
    .Then(state =>
    {
        var mike = state.GetAgents().First();
        Assert.That(mike.PositionComponent.CurrentPosition.TileId, Is.EqualTo(targetTile));
    });
```

### Chained Commands

```csharp
ScenarioRunner.Create()
    .GivenMission(TestHelpers.Mission01Json)
    .GivenSurvivors("Survivor.Mike")
    .When(cmd => cmd.Move("Survivor.Mike").To(tile1))
    .When(cmd => cmd.Move("Survivor.Mike").To(tile2))
    .When(cmd => cmd.Move("Survivor.Mike").To(tile3))
    .Then(state =>
    {
        // Validate final state
    });
```

## ScenarioRunner API

### Setup Methods

**`GivenMission(string missionJson)`**  
Initializes the game board from mission JSON. Automatically spawns props and sets up the spatial model.

**`GivenSurvivors(params string[] survivorIds)`**  
Spawns survivors at their designated spawn points.

**`GivenAgents(params SpawnRequest[] requests)`**  
Spawns agents with custom spawn requests for more control.

### Action Methods

**`When(Func<CommandBuilder, IActionCommand> commandAction)`**  
Executes a command built using the fluent CommandBuilder.

**`WhenCommand(IActionCommand command)`**  
Executes a pre-built command directly.

### Assertion Methods

**`Then(Action<GameState> assertion)`**  
Validates the current game state using custom assertions.

**`ThenEffects(Action<IReadOnlyList<IGameEffect>> assertion)`**  
Validates the effects captured from the last command execution.

### Helper Methods

**`GetCurrentState()`**  
Returns the current GameState for custom validation.

**`GetCapturedEffects()`**  
Returns effects captured from the last command execution.

## CommandBuilder API

### Move Commands

```csharp
// Using TileId
cmd.Move("Survivor.Mike").To(tileId)

// Using GUID
cmd.Move("Survivor.Mike").To(guid)

// Using Position
cmd.Move("Survivor.Mike").To(position)
```

## ScenarioSerializer API

### JSON Scenario Format

```json
{
  "name": "Move After Spawn",
  "description": "Validates survivor can move after spawning",
  "setup": {
    "missionJson": "...",
    "survivors": ["Survivor.Mike", "Survivor.Doug"]
  },
  "commands": [
    {
      "type": "Move",
      "agentId": "Survivor.Mike",
      "targetTileId": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53"
    }
  ],
  "assertions": [
    {
      "type": "AgentPosition",
      "agentId": "Survivor.Mike",
      "expectedTileId": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53"
    }
  ]
}
```

### Loading & Executing Scenarios

```csharp
// Load from file
var scenario = ScenarioSerializer.LoadScenario("Scenarios/move_basic.json");

// Execute
var finalState = ScenarioSerializer.ExecuteScenario(scenario);

// Save scenario
ScenarioSerializer.SaveScenario(scenario, "Scenarios/my_scenario.json");
```

## Examples

See `Examples/ScenarioRunnerExamples.cs` for comprehensive usage examples including:
- Basic initialization
- Movement commands
- Chained commands
- Effect validation

## Benefits

✅ **Readable** - Given/When/Then pattern makes intent clear  
✅ **Composable** - Chain commands and assertions fluently  
✅ **Reusable** - JSON scenarios can be version-controlled  
✅ **Fast** - No UI overhead, pure engine testing  
✅ **Maintainable** - Changes to test structure isolated to helpers
