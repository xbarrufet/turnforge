using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.GameState;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions;
using Parchis.Rules.Commands;
using Parchis.Rules.Board;
using System.Linq;

namespace Parchis.Rules.Fsm;

/// <summary>
/// Factory to create the Parchís FSM using the builder pattern.
/// </summary>
public static class ParchisFsmFactory
{
    /// <summary>
    /// Creates a GameFsm V2 configured for Parchís.
    /// </summary>
    public static GameFsm CreateParchisFsm(GameState initialState, IGameLogger? logger = null)
    {
        var players = initialState.Players.Keys.ToArray();
        
        var round = Round.For(players)
            .WithPhases(new Phases.ParchisTurnPhase())
            .Build();

        return GameFsm.Build(
            round, 
            completionCondition: state => false, // TODO: Implement victory check
            logger
        );
    }
}
