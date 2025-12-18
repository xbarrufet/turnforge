using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Infrastructure.Appliers.Interfaces;

public interface IAppplierResult
{
    public GameState GameState { get; }
    public IGameEffect GameEffect { get; }
}