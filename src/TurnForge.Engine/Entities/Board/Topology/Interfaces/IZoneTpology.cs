

using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Topology.Interfaces
{
    public interface IZoneTopology
    {
        bool IsInsideZone(IBoardPositionId position);

        bool CanTraverse(IBoardPositionId from, IBoardPositionId to);

        int Distance(IBoardPositionId from, IBoardPositionId to);
    
        public static IZoneTopology Empty => new TileGraph();
        
        public TopologyKind Kind { get;  }
    }
    
}