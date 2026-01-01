using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities; // For GameEntity.Descriptors.Interfaces;

namespace TurnForge.Engine.Definitions.Descriptors.Interfaces;

public interface IGameEntityDescriptor<T> where T : GameEntity;