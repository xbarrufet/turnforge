using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IUpdateApplier<in TDecision, TComponent> : IApplier<TDecision>
    where TDecision : IUpdateDecision<TComponent>
    where TComponent : IGameEntityComponent
{
}
