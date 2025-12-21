using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IApplierResponse
{
    public GameState GameState { get; }
    public IGameEffect GameEffect { get; }
}