using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Components;

public class ZoneEffectComponent(List<IZoneBehaviour> behaviours) : IGameEntityComponent
{
    public IReadOnlyList<IZoneBehaviour> Behaviours { get; } = behaviours;
}
