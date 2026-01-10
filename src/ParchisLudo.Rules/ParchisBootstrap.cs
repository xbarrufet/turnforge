using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Rules;

/// <summary>
/// Action IDs for Parchis game-specific actions.
/// For core actions (StartGame, Spawn), use TurnForge.Engine.Core.Actions.CoreActions.
/// </summary>
public static class ParchisActions
{
    public static readonly ActionId Move = new("parchis_move");
}