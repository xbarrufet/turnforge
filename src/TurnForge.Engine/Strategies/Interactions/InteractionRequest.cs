namespace TurnForge.Engine.Strategies.Interactions;

/// <summary>
/// Request sent to UI when a strategy suspends for user input.
/// </summary>
/// <remarks>
/// The UI should display appropriate controls based on Type and call
/// SubmitInteractionCommand with the user's response.
/// 
/// Common Types:
/// - "DiceRoll": User clicks to roll dice
/// - "SpendResource": User chooses to spend resource or not
/// - "YesNo": Simple confirmation
/// - "SelectOption": Multiple choice selection
/// </remarks>
public record InteractionRequest
{
    /// <summary>
    /// Session ID to correlate request with response.
    /// </summary>
    public Guid SessionId { get; init; }
    
    /// <summary>
    /// Type of interaction (e.g., "DiceRoll", "SpendResource", "YesNo").
    /// UI uses this to determine which control to display.
    /// </summary>
    public string Type { get; init; } = string.Empty;
    
    /// <summary>
    /// Human-readable prompt for the player.
    /// </summary>
    public string Prompt { get; init; } = string.Empty;
    
    /// <summary>
    /// Type-specific metadata (e.g., min/max for dice, options for select).
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
    
    /// <summary>
    /// ID of the agent performing the action (for UI display).
    /// </summary>
    public string? AgentId { get; init; }
}
