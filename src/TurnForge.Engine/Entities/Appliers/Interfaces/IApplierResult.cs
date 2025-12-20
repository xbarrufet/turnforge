using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IApplierResult
{
    public GameState GameState { get; }
    public IGameEffect GameEffect { get; }
}