namespace TurnForge.Engine.Core.Action.Interfaces;

/// <summary>
/// Marker interface for typed workflow data.
/// Implement this to create strongly-typed context data for workflows.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// public record RollDiceData(int Die1, int Die2, int Total) : IActionData;
/// 
/// // In node:
/// context.SetTypedData(new RollDiceData(1, 2, 3));
/// var data = context.GetTypedData&lt;RollDiceData&gt;();
/// </code>
/// </remarks>
public interface IActionData
{
}
