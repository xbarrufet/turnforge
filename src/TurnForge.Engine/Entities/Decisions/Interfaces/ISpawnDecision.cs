using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Decisions.Interfaces;

public interface ISpawnDecision : IBuildDecision<Prop>
{
    Position Position { get; }
    IReadOnlyList<IActorBehaviour> ExtraBehaviours { get; }
}