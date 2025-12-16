

using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Spatial.Interfaces
{
    public interface ISpatialModel
    {
        bool IsValidPosition(Position position);

        IEnumerable<Position> GetNeighbors(Position position);

        bool CanMove(Actor actor, Position target);

        int Distance(Position from, Position to);
        
        void EnableConnection(Position from, Position to);
        
        void DisableConnection(Position from, Position to);
        
    }
}