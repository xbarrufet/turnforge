using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Attack;

/// <summary>
/// Command to perform an attack action.
/// </summary>
/// <remarks>
/// This is a generic attack command. The actual combat logic
/// is implemented in game-specific IActionStrategy implementations.
/// 
/// WeaponId is optional - null means unarmed attack.
/// </remarks>
public sealed record AttackCommand(
    string attackerId, 
    string targetId, 
    string? weaponId = null,
    bool hasCost = true) : IActionCommand
{
    public Type CommandType => typeof(AttackCommand);
    
    /// <summary>
    /// The entity performing the attack.
    /// </summary>
    public string AgentId { get; set; } = attackerId;
    
    /// <summary>
    /// The target entity being attacked.
    /// </summary>
    public string TargetId { get; set; } = targetId;
    
    /// <summary>
    /// Optional weapon used for the attack. Null = unarmed.
    /// </summary>
    public string? WeaponId { get; set; } = weaponId;
    
    /// <summary>
    /// Whether this action costs Action Points.
    /// </summary>
    public bool HasCost { get; set; } = hasCost;
}
