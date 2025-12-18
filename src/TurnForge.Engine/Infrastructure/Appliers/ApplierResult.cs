using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Infrastructure.Appliers;

public sealed class ApplierResult(GameState gameState, IGameEffect gameEffect) : Interfaces.IAppplierResult
{
    public GameState GameState { get; } = gameState;
    public IGameEffect GameEffect { get; } = gameEffect;
}