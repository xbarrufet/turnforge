

using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Board.Interfaces;


public interface IZoneBound
{
    bool Contains(Position position);
}
