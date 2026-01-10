using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Rules.Actions;

public class MoveActionContext : ActionContext
{

    public bool MovePawnFromSpawn
    {
        get => TryGet<bool>(nameof(MovePawnFromSpawn), out var v) && v;
        set => Set(nameof(MovePawnFromSpawn), value);
    }

    public GameEntity SelectedPawn
    {
        get => TryGet<Actor>(nameof(SelectedPawn), out var v) ? v : GameEntity.Emtpy;
        set => Set(nameof(SelectedPawn), value);
    }




}