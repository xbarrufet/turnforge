using TurnForge.Engine.Definitions.Actors;

namespace TurnForge.Engine.Definitions.Interfaces;

public interface IReadOnlyGameState
{
    IReadOnlyCollection<Agent> Agents { get; }
    IReadOnlyCollection<Prop> Props { get; }

}