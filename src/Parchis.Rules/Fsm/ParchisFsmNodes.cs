using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Fsm.Builders;
using TurnForge.Engine.Definitions;
using Parchis.Rules.Commands;
using Parchis.Rules.Board;

namespace Parchis.Rules.Fsm;

/// <summary>
/// Factory to create the Parchís FSM using the builder pattern.
/// </summary>
public static class ParchisFsmFactory
{
    /// <summary>
    /// Create the FSM sequence using the builder.
    /// </summary>
    public static IEnumerable<FsmNode> CreateSequence()
    {
        return FsmBuilder.Create()
            // Roll dice phase - waits for RollDiceCommand
            .Phase("RollDice", 
                isCompleted: state => state.Metadata.ContainsKey("DiceRolled") && (bool)state.Metadata["DiceRolled"],
                typeof(RollDiceCommand))
            
            // Move piece phase - waits for MovePieceCommand or PassTurnCommand
            .Phase("MovePiece",
                isCompleted: state => state.Metadata.ContainsKey("MoveCompleted") && (bool)state.Metadata["MoveCompleted"],
                typeof(MovePieceCommand), typeof(PassTurnCommand))
            
            // Check victory - pass-through
            .Phase("CheckVictory",
                isCompleted: _ => true)
            
            // Next player - pass-through
            .Phase("NextPlayer",
                isCompleted: _ => true)
            
            .Build();
    }
    
    /// <summary>
    /// Create an FsmController for Parchís.
    /// </summary>
    public static FsmController CreateController()
    {
        return FsmBuilder.Create()
            .Phase("RollDice", 
                isCompleted: state => state.Metadata.ContainsKey("DiceRolled") && (bool)state.Metadata["DiceRolled"],
                typeof(RollDiceCommand))
            .Phase("MovePiece",
                isCompleted: state => state.Metadata.ContainsKey("MoveCompleted") && (bool)state.Metadata["MoveCompleted"],
                typeof(MovePieceCommand), typeof(PassTurnCommand))
            .Phase("CheckVictory", isCompleted: _ => true)
            .Phase("NextPlayer", isCompleted: _ => true)
            .BuildController();
    }
}
