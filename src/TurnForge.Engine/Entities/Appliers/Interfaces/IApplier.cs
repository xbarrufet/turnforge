using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IApplier<in TDecision> where TDecision : IDecision
{
    ApplierResponse Apply(TDecision decision, GameState state);
}
