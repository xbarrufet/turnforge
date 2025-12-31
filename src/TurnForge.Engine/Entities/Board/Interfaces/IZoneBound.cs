

using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Board.Interfaces;


public interface IZoneBound
{
    bool Contains(IBoardPosition position);
}
