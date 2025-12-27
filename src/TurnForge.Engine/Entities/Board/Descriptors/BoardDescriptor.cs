using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Definitions.Descriptors.Interfaces;
using TurnForge.Engine.Spatial.Interfaces;

namespace TurnForge.Engine.Definitions.Board.Descriptors;


public sealed record BoardDescriptor(SpatialDescriptor Spatial, IReadOnlyList<ZoneDescriptor> Zones) : IGameEntityDescriptor<GameBoard>;