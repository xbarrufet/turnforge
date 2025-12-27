using TurnForge.Engine.Definitions;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Appliers.Entity.Interfaces;

public interface IApplier<in TDecision> where TDecision : IDecision
{
    ApplierResponse Apply(TDecision decision, GameState state);
}
