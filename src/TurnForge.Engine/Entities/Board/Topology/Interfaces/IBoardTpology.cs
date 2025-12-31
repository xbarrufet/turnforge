

using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Topology.Interfaces
{
    public interface IBoardTopology
    {
        bool IsValidPosition(IBoardPosition position);

        bool CanTraverse(IBoardPosition from, IBoardPosition to);

        int Distance(IBoardPosition from, IBoardPosition to);


    }
}