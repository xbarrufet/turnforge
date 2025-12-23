using TurnForge.Engine.Entities;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Appliers.Entity.Interfaces;

public interface IUpdateApplier<in TDecision, TComponent> : IApplier<TDecision>
    where TDecision : IUpdateDecision<TComponent>
    where TComponent : IGameEntityComponent
{
}
