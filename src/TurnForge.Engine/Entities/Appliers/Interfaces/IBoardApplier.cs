using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IBoardApplier
{
    GameBoard Apply(SpatialDescriptor spatial, IEnumerable<ZoneDescriptor> zones);
}