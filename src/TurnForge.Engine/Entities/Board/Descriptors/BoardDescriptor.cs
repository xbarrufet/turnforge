using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Spatial.Interfaces;

namespace TurnForge.Engine.Entities.Board.Descriptors;


public sealed record BoardDescriptor(SpatialDescriptor Spatial, IReadOnlyList<ZoneDescriptor> Zones) : IGameEntityDescriptor<GameBoard>;