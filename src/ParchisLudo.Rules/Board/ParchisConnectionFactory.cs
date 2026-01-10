namespace ParchisLudo.Rules.Board;
/*
/// <summary>
/// Factory to create Parchis connection props with metadata.
/// Connection props represent special transitions between board locations.
/// </summary>
public static class ParchisConnectionFactory
{
    /// <summary>
    /// Creates all Parchis connection props including finish entry connections.
    /// </summary>
    public static List<ConnectionDeployment> CreateConnections()
    {
        var connections = new List<ConnectionDeployment>();

        // Cell → Final zone colors (trait: Color)
        // These are the special connections from main track to finish lanes
        connections.Add(CreateFinishEntryConnection("red", ParchisBoard.RedFinishEntry, "red_finish_1"));
        connections.Add(CreateFinishEntryConnection("blue", ParchisBoard.BlueFinishEntry, "blue_finish_1"));
        connections.Add(CreateFinishEntryConnection("green", ParchisBoard.GreenFinishEntry, "green_finish_1"));
        connections.Add(CreateFinishEntryConnection("yellow", ParchisBoard.YellowFinishEntry, "yellow_finish_1"));

        return connections;
    }

    private static ConnectionDeployment CreateFinishEntryConnection(
        string colorName,
        string fromTile,
        string toTile)
    {
        // Convert color name to PlayerColor enum
        var color = colorName.ToLower() switch
        {
            "red" => ParchisBoard.PlayerColor.Red,
            "blue" => ParchisBoard.PlayerColor.Blue,
            "green" => ParchisBoard.PlayerColor.Green,
            "yellow" => ParchisBoard.PlayerColor.Yellow,
            _ => throw new ArgumentException($"Invalid color: {colorName}")
        };

        var descriptor = new ConnectionDescriptor(
            FinishLinitConnectionDefinition.DefId,
            extraComponents: null,
            definitionTraitValues: new ITrait[]
            {
                new ColorTrait(color)  // Provide color value for this specific connection
            }
        );

        // Position is the "from" tile
        var position = new TilePosition(new TileId(fromTile));

        return new ConnectionDeployment(descriptor, position);
    }
}*/
