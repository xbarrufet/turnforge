# Steps to create a parchis simulator usinf TurnForge

## 1. Create all Definitions, Events, Traits and Reactions


### 1. Traits
- **ColorTrait;** defines the color of the player or agent
``` charp
public class ColorTrait(PlayerColor color) : BaseDataTrait
{
    public PlayerColor Color { get; init; } = color;
}
```

- **SafeZoneTrait;** defines a safe zone in the board
``` charp
public class SafeZoneTrait(bool safe=false) : BaseDataTrait
{
    public bool Safe { get; init; } = safe;
}
```

- **BlockZoneTrait;** defines a block zone in the board-> two pawns of the same color in the same cell
``` charp
public class BlockZoneTrait(bool block=false) : BaseDataTrait
{
    public bool Block { get; init; } = block;
}
```

- **PlayerDefinition;** defines the player. **THIS IS ALWAYS NEEDED**
``` charp
public class ParchisPlayerDefinition : PlayerDefinition
{
    public PlayerColor Color { get; }

    public ParchisPlayerDefinition(string definitionId, PlayerId playerId, PlayerColor color) 
        : base(definitionId, playerId)
    {
        Color = color;
        AddTrait(new ColorTrait(color));
    }
}
```

- **AgentDefinition;** defines the pawn agent.
``` charp
public class ParchisPawnDefinition : AgentDefinition
{
    public PlayerColor Color { get; }

    public ParchisPawnDefinition(string definitionId, PlayerId playerId, PlayerColor color) 
        : base(definitionId, playerId)
    {
        Color = color;
        AddTrait(new ColorTrait(color));
    }
}
```
- **ZoneDefinition;** defines the zone where the pawn can move, is linked to a tile but is different from the board tile as we need to add traits.
``` charp
public class SecureCellDefinition : ZoneDefinition  {
    public SecureCellDefinition(string definitionId, TileId tileId) : 
                base(definitionId, "Cell", 
                new TileSetZoneBound(Position.FromTile(tileId))) {
        AddTrait(new SafeZoneTrait());
        AddTrait(new BlockZoneTrait());
    }
}
```

## 2. Create the Board logic
Parchis is a tiled board we will use the discrete spatial model to create it. 
Create a list connections of TileId, representing the connections between tiles (back and forth as we have also to go back if we face a block zone or in the final color zone).

```csharp
  var connections = new List<(TileId, TileId)>();
        
        // Main circuit: 0-67 connected sequentially
        for (int i = 1; i < ParchisBoard.MainCircuitSize-1; i++)
        {
            connections.Add((new TileId($"track_{i}"), new TileId($"track_{i + 1}")));
            if(i>1)
            {
                connections.Add((new TileId($"track_{i}"), new TileId($"track_{i - 1}")));
            }
        }
        
        // Add finish lines
        AddFinishLaneConnections(connections, "yellow", ParchisBoard.YellowFinishEntry);
        // add starting areas
        AddSpawnAreas(connections);
```
return a graph from the connections list
``` csharp
var graph = new MutableTileGraph(connections);
var spatialModel = new ConnectedGraphSpatialModel(graph);
return spatialModel;
```

### 3. Create StartGame Parameters

`StartGame` is the core action to initialize a game session with players, board, zones, connections, and mission data.

#### StartGameParams Structure

```csharp
public record StartGameParams(
    List<AddPlayerInput> PlayerInputs,      // Player configuration
    List<PropDeploymentInput> PropInputs,   // Props to deploy
    BoardDataInput BoardData,                // Board topology + zones + connections
    MissionDataInput MissionData             // Mission configuration
) : IActionParameters;
```

#### BoardDataInput Components

```csharp
public record BoardDataInput(
    string MapId,                                          // Map identifier
    IBoardDefinition BoardDefinition,                      // Board topology (graph)
    IReadOnlyList<BoardZoneDefinition> Zones,             // Zones with traits
    IReadOnlyList<BoardConnectionDefinition> Connections  // Connection props
) : IActionInput;
```

#### Creating Board Data for Parchis

**1. Board Topology (Graph)**:
```csharp
var boardDef = ParchisBoardFactory.CreateDescriptor();
// Creates:
// - Main circuit connections (bidirectional)
// - Finish lane connections
// - Spawn to entry connections
```

**2. Zones with Traits**:
```csharp
var zones = ParchisZoneFactory.CreateZones();
// Creates:
// - 4 Spawn zones (ColorTrait + SpawnZoneTrait)
// - 4 Entry cells (SafeZoneTrait)
// - 8 Safety zones (SafeZoneTrait)
// - 1 Center zone (CenterTrait)
```

**3. Connection Props**:
```csharp
var connections = ParchisConnectionFactory.CreateConnections();
// Creates:
// - 4 Finish entry connections with color restrictions
```

#### Complete Parchis StartGame Example

```csharp
// 1. Create player inputs
var playerInputs = new List<AddPlayerInput>();
foreach (var (playerId, color) in playerColors)
{
    var agentInputs = new List<AgentDeploymentInput>();
    for (int i = 0; i < 4; i++)
    {
        var desc = new AgentDescriptor("pawn", color, color);
        agentInputs.Add(new AgentDeploymentInput(desc, null));
    }
    playerInputs.Add(new AddPlayerInput(
        playerId, 
        PlayerControllerType.AI, 
        color, 
        color, 
        IActionPool.FixAmount, 
        1, 
        agentInputs
    ));
}

// 2. Create board data
var boardDef = ParchisBoardFactory.CreateDescriptor();
var zones = ParchisZoneFactory.CreateZones();
var connections = ParchisConnectionFactory.CreateConnections();

var boardInput = new BoardDataInput(
    "parchis_standard",
    boardDef,
    zones,
    connections
);

// 3. Create mission data
var missionData = new MissionDataInput("parchis_standard");

// 4. Execute StartGame
var startParams = new StartGameParams(
    playerInputs,
    new List<PropDeploymentInput>(),  // No props in Parchis
    boardInput,
    missionData
);

var result = GameEngineExtensions.ExecuteAction(
    engine, 
    CoreActions.StartGameActionId, 
    startParams
);
```

#### Zone and Connection Factory Patterns

**Zone Factory Example**:
```csharp
public static class ParchisZoneFactory
{
    public static List<BoardZoneDefinition> CreateZones()
    {
        var zones = new List<BoardZoneDefinition>();
        
        // Spawn zones with traits
        zones.Add(CreateSpawnZone(PlayerColor.Red, "spawn_red"));
        
        // Safe zones
        zones.Add(CreateSafeZone("red_entry", "track_39"));
        
        // Center zone
        zones.Add(CreateCenterZone());
        
        return zones;
    }
    
    private static BoardZoneDefinition CreateSpawnZone(PlayerColor color, string tileId)
    {
        var descriptor = new ZoneDescriptor(
            $"zone_spawn_{color.ToString().ToLowerInvariant()}",
            extraComponents: null,
            requestedTraits: new IDataTrait[]
            {
                new ColorTrait(color),
                new SpawnZoneTrait()
            }
        );
        return new BoardZoneDefinition(descriptor, new TilePosition(new TileId(tileId)));
    }
}
```

**Connection Factory Example**:
```csharp
public static class ParchisConnectionFactory
{
    public static List<BoardConnectionDefinition> CreateConnections()
    {
        var connections = new List<BoardConnectionDefinition>();
        
        // Finish entry connections with color restrictions
        connections.Add(CreateFinishEntryConnection("red", "track_50", "red_finish_1"));
        
        return connections;
    }
    
    private static BoardConnectionDefinition CreateFinishEntryConnection(
        string color, 
        string fromTile, 
        string toTile)
    {
        var descriptor = new ConnectionDescriptor(
            new TileId(fromTile),
            new TileId(toTile),
            $"finish_entry_{color}",  // Category
            color  // RestrictedToTeam
        );
        
        var position = new TilePosition(new TileId(fromTile));
        return new BoardConnectionDefinition(descriptor, position);
    }
}
```