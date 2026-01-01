using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Entities.Actors; // For Agent, Prop

namespace TurnForge.Engine.Definitions.Interfaces;

public interface IReadOnlyGameState
{
    IReadOnlyCollection<Agent> Agents { get; }
    IReadOnlyCollection<Prop> Props { get; }

}