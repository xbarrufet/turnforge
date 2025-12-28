namespace TurnForge.Engine.Core.Interfaces;

/// <summary>
/// Marker interface for payloads describing an Input Request from the Workflow Engine to the UI/User.
/// Examples: DiceRollRequest, SelectionRequest, etc.
/// </summary>
public interface IInputRequest
{
    // Marker interface. Specific requests will have properties.
    // e.g. "Description", "ValidationRules", etc.
}
