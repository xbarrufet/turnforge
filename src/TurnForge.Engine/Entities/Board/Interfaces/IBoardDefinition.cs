using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IBoardDefinition
{
    BoardKind Kind { get; }
    IReadOnlyList<BoardPropDefinition>? Props { get; }
}

public record BoardPropDefinition(
    TurnForge.Engine.Entities.Definitions.Actors.PropDefinition Definition,
    IBoardPosition FixedPosition
);