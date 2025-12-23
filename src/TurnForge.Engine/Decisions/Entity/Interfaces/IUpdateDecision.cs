using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Entities.Decisions.Interfaces;

public interface IUpdateDecision<T> : IDecision where T : IGameEntityComponent
{
}