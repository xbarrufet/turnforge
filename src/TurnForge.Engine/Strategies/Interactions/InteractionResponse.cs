namespace TurnForge.Engine.Strategies.Interactions;

/// <summary>
/// Response from UI providing user input for a suspended strategy.
/// </summary>
public record InteractionResponse
{
    /// <summary>
    /// Session ID matching the original InteractionRequest.
    /// </summary>
    public Guid SessionId { get; init; }
    
    /// <summary>
    /// User-provided input data.
    /// Keys depend on the interaction Type (e.g., "Roll" for DiceRoll).
    /// </summary>
    public Dictionary<string, object> Data { get; init; } = new();
    
    /// <summary>
    /// Whether the user cancelled the interaction.
    /// If true, the strategy should abort without applying changes.
    /// </summary>
    public bool Cancelled { get; init; }
}
