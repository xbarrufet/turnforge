using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Core.Action.Interfaces;

public interface IActionContextFactory
{
    TActionContext BuildActioContrxt<TActionContext>(GameState baseState) where TActionContext : ActionContext;
    
}