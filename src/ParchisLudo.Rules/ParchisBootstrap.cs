using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Actions;

namespace Parchis.Rules;

/// <summary>
/// Action IDs for Parchis game.
/// Use these when calling ExecuteAction from UI.
/// </summary>
public static class ParchisActions
{
    public static readonly ActionId Move = new("parchis_move");
    public static readonly ActionId StartGame = new("parchis_game_start");
}