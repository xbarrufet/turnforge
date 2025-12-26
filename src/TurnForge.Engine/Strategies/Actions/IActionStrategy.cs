using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Strategy interface for executing action commands.
/// </summary>
/// <typeparam name="TCommand">Type of action command this strategy handles</typeparam>
/// <remarks>
/// Strategies contain the validation and business logic for actions.
/// They receive a command and context, and return a result with decisions.
/// 
/// Supports two execution modes:
/// - Fast Track: Return Completed/Failed immediately (simple logic)
/// - Interactive: Return Suspended with InteractionRequest (user input needed)
/// 
/// Responsibilities:
/// - Validate the action can be performed
/// - Calculate costs (AP, resources, etc.)
/// - Build decisions for component updates
/// - Return metadata for UI feedback
/// - Optionally suspend for user input (dice rolls, decisions)
/// 
/// Strategies are STATELESS and injected via DI.
/// Use ActionContext.Variables for transient pipeline state.
/// </remarks>
public interface IActionStrategy<in TCommand> where TCommand : IActionCommand
{
    /// <summary>
    /// Execute the strategy for the given command.
    /// </summary>
    /// <param name="command">Action command to execute</param>
    /// <param name="context">Game context (state, board, session data)</param>
    /// <returns>Strategy result (Completed/Suspended/Failed)</returns>
    StrategyResult Execute(TCommand command, ActionContext context);
}
