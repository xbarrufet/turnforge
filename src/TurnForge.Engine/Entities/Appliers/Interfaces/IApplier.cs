using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IApplier<in TDecision> where TDecision : IDecision
{
    GameState Apply(TDecision decision, GameState state);
}
