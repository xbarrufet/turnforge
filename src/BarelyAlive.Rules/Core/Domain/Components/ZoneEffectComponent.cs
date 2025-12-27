using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Components;

public class ZoneEffectComponent(List<IZoneTrait> traits) : IGameEntityComponent
{
    public IReadOnlyList<IZoneTrait> Traits { get; } = traits;
}
