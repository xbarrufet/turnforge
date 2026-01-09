using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action;

/// <summary>
/// Represents a pending agent deployment with descriptor and optional position.
/// Position is mutable to allow mission rules to resolve it.
/// </summary>
public class AgentDeployment
{
    public required IGameEntityBuildDescriptor Descriptor { get; init; }
    public required PlayerId OwnerId { get; init; }
    
    /// <summary>
    /// Deploy position. Can be:
    /// - Explicit (Kill Team style)
    /// - Null initially, resolved by mission rules (Zombicide style)
    /// </summary>
    public IBoardPosition? Position { get; set; }
}

/// <summary>
/// Represents a pending prop deployment with definition and fixed position.
/// </summary>
public record PropDeployment(
    BaseGameEntityDefinition Definition,
    IBoardPosition Position
);
