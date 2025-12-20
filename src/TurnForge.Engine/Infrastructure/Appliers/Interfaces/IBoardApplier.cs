using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;

namespace TurnForge.Engine.Infrastructure.Appliers;

public interface IBoardApplier
{
    GameBoard Apply(SpatialDescriptor spatial, IEnumerable<ZoneDescriptor> zones);
}