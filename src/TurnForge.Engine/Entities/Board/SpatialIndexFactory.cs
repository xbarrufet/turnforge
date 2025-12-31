using TurnForge.Engine.Domain.Board.Spatial.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.Entities.Board;

public sealed class SpatialIndexFactory : ISpatialIndexFactory
{
    public ISpatialIndex CreateSpatialIndex()
    {
        return new SpatialIndex();
    }

    
}