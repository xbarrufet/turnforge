# Data Formats Reference

How to define game data in TurnForge.

---

## Entity Definitions

Entities are defined using `BaseGameEntityDefinition` with traits.

### Code Definition

```csharp
var zombie = new BaseGameEntityDefinition("Zombie.Walker", "Enemy")
    .AddTrait(new IdentityTrait("Zombie"))
    .AddTrait(new HealthTrait(3))
    .AddTrait(new MovementTrait(1))
    .AddTrait(new AttackTrait(1, Range: 1));

catalog.RegisterDefinition(zombie);
```

### JSON Definition

```json
{
  "definitionId": "Zombie.Walker",
  "category": "Enemy",
  "traits": [
    { "type": "Identity", "category": "Zombie" },
    { "type": "Health", "max": 3 },
    { "type": "Movement", "speed": 1 },
    { "type": "Attack", "damage": 1, "range": 1 }
  ]
}
```

**Loading:**
```csharp
var definitions = JsonSerializer.Deserialize<List<EntityDefinitionDto>>(json);
foreach (var dto in definitions)
{
    var def = DefinitionFactory.FromDto(dto);
    catalog.RegisterDefinition(def);
}
```

---

## Mission Data

Missions define what to spawn and game objectives.

```json
{
  "missionId": "Tutorial_01",
  "name": "First Steps",
  "board": "Board_Tutorial",
  "spawns": [
    {
      "definitionId": "Survivor.Rick",
      "position": { "x": 5, "y": 5 },
      "team": "Player"
    },
    {
      "definitionId": "Zombie.Walker",
      "position": { "x": 10, "y": 10 },
      "count": 3,
      "team": "Zombie"
    }
  ],
  "objectives": [
    { "type": "KillAll", "team": "Zombie" },
    { "type": "Survive", "turns": 10 }
  ]
}
```

---

## Board Data

Boards define the playable area.

```json
{
  "boardId": "Board_Tutorial",
  "width": 20,
  "height": 15,
  "tiles": [
    { "x": 0, "y": 0, "type": "Floor" },
    { "x": 1, "y": 0, "type": "Wall", "blocking": true },
    { "x": 5, "y": 5, "type": "SpawnPoint", "team": "Player" }
  ],
  "zones": [
    { "id": "SafeZone", "tiles": [[0,0], [0,1], [1,0], [1,1]] }
  ]
}
```

---

## Spawn Requests

Runtime spawn commands:

```csharp
var requests = new List<SpawnRequest>
{
    new SpawnRequest("Zombie.Walker")
    {
        Position = new Position(10, 10),
        Count = 5,
        Team = "Zombie"
    }
};

var command = new SpawnAgentsCommand(requests);
engine.SendCommand(command);
```

---

## Trait Registry

To enable JSON loading, register trait types:

```csharp
TraitRegistry.Register<HealthTrait>("Health");
TraitRegistry.Register<MovementTrait>("Movement");
TraitRegistry.Register<AttackTrait>("Attack");
TraitRegistry.Register<ExplodeOnWalkOverTrait>("ExplodeOnWalkOver");
```

---

## File Structure Convention

```
GameData/
├── Definitions/
│   ├── Survivors.json
│   ├── Zombies.json
│   └── Props.json
├── Missions/
│   ├── Tutorial_01.json
│   └── Campaign_01.json
└── Boards/
    ├── Board_Tutorial.json
    └── Board_City.json
```
