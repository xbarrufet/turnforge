using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action;

/// <summary>
/// Represents a pending agent deployment with descriptor and position.
/// </summary>
public class AgentDeployment
{
    public required IGameEntityBuildDescriptor Descriptor { get; init; }
    public required PlayerId OwnerId { get; init; }
    public required string Team { get; init; }
    public required IBoardPosition Position { get; init; }
}

/// <summary>
/// Represents a pending prop deployment with definition and fixed position.
/// </summary>
public record PropDeployment(
    BaseGameEntityDefinition Definition,
    IBoardPosition Position
);

public record ZoneDeployment(
    ZoneDescriptor Zone,
    IBoardPosition Position
);

public record ConnectionDeployment(
    ConnectionDescriptor Descriptor,
    IBoardPosition Position
);
