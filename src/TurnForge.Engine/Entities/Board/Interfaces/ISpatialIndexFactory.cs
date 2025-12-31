using TurnForge.Engine.Domain.Board.Spatial.Interfaces;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface ISpatialIndexFactory
{
    ISpatialIndex CreateSpatialIndex();
}