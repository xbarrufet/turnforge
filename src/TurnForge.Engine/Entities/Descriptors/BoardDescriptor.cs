using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Entities.Board;

public record struct BoardDescriptor(
    TopologyKind Kind,
    IReadOnlyList<ZoneDescriptor> Zones,
    IReadOnlyList<ConnectionDescriptor> Connections);
    