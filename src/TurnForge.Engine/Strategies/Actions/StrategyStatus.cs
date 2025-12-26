namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Status of a strategy execution.
/// </summary>
public enum StrategyStatus
{
    /// <summary>
    /// Strategy completed successfully. Decisions can be applied.
    /// </summary>
    Completed,
    
    /// <summary>
    /// Strategy is suspended waiting for user input.
    /// UI should display the InteractionRequest and resume with SubmitInteractionCommand.
    /// </summary>
    Suspended,
    
    /// <summary>
    /// Strategy failed validation. See ValidationErrors for details.
    /// </summary>
    Failed
}
