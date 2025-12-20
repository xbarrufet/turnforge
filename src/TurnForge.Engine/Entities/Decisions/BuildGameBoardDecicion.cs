using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Entities.Decisions;

public sealed record BuildGameBoardDecicion(SpatialDescriptor Spatial, IEnumerable<ZoneDescriptor> Zones) : IUpdateDecision;
