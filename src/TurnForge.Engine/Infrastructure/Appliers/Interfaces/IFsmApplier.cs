using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Infrastructure.Appliers.Interfaces;

public interface IFsmApplier
{
    GameState Apply(GameState state, IEffectSink effectSink);
}