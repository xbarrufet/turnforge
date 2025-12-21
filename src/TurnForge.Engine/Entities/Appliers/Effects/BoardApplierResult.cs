using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers.Effects;

public sealed record BoardApplierResult(EntityId BoardId, IReadOnlyList<ZoneDefinition> Zones) : IGameEffect;