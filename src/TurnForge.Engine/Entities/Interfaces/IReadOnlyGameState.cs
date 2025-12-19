using TurnForge.Engine.Entities.Actors;

namespace TurnForge.Engine.Entities.Interfaces;

public interface IReadOnlyGameState
{
    IReadOnlyCollection<Agent> Agents { get; }
    IReadOnlyCollection<Prop> Props { get; }

}