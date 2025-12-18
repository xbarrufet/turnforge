

using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;


public interface IZoneBound
{
    bool Contains(Position position);
}
