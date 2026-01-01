using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Actions;

namespace Parchis.Rules;

/// <summary>
/// Action IDs for Parchis game-specific actions.
/// For core actions (StartGame, Spawn), use TurnForge.Engine.Core.Actions.CoreActions.
/// </summary>
public static class ParchisActions
{
    public static readonly ActionId Move = new("parchis_move");
}