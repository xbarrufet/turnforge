using TurnForge.Engine.Entities.Appliers.Interfaces;

namespace TurnForge.Engine.Entities.Decisions.Interfaces;

public interface IBuildDecision<T> : IDecision where T : GameEntity;
