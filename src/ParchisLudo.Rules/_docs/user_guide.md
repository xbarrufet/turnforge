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

### 3. Creates Spawn Strategy
*InitGame* and *StartGame* are the system commands we have to use to create and start the game
- **InitGame** is used to create the game and initialize the board
```csharp
public sealed record InitGameCommand(
    BoardDescriptor Board,
    IReadOnlyList<SpawnRequest> Players,
    IReadOnlyList<SpawnRequest>? Props = null
) : ICommand
```
- **StartGame** is used to start the game and initialize the players
        