using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Move;

public sealed record MoveCommand(string agentId, bool hasCost, Position targetPosition) : IActionCommand
{
    
    public Type CommandType => typeof(MoveCommand);

    public string AgentId { get; set; } = agentId;
    public Position TargetPosition { get; set; } = targetPosition;

    public bool HasCost { get; set; } = hasCost;
}   