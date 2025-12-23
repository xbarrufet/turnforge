using TurnForge.Engine.Entities;
using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Decisions.Entity.Interfaces;

public interface IUpdateDecision<T> : IDecision where T : IGameEntityComponent
{
}