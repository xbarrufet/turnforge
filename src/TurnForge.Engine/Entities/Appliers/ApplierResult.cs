using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Entities.Appliers;

public sealed class ApplierResult(GameState gameState, IGameEffect gameEffect) : Interfaces.IApplierResult
{
    public GameState GameState { get; } = gameState;
    public IGameEffect GameEffect { get; } = gameEffect;
}