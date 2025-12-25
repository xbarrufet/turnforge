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
/// Responsibilities:
/// - Validate the action can be performed
/// - Calculate costs (AP, resources, etc.)
/// - Build decisions for component updates
/// - Return metadata for UI feedback
/// 
/// Strategies are STATELESS and injected via DI.
/// </remarks>
public interface IActionStrategy<in TCommand> where TCommand : IActionCommand
{
    /// <summary>
    /// Execute the strategy for the given command.
    /// </summary>
    /// <param name="command">Action command to execute</param>
    /// <param name="context">Game context (state, board)</param>
    /// <returns>Strategy result (success/failure with decisions and metadata)</returns>
    ActionStrategyResult Execute(TCommand command, IActionContext context);
}
