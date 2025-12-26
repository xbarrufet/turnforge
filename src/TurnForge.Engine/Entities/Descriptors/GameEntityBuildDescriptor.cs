using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

/// <summary>
/// Base class for entity build descriptors.
/// Provides common properties needed for entity creation.
/// </summary>
public class GameEntityBuildDescriptor(string definitionId) : IGameEntityBuildDescriptor
{
    public string DefinitionID { get; set; } = definitionId;
    public List<IGameEntityComponent> ExtraComponents { get; init; } = new();
    
    /// <summary>
    /// Position where the entity will spawn.
    /// Use Position.Empty if position should be determined by spawn strategy.
    /// Maps to PositionComponent.CurrentPosition via PropertyAutoMapper.
    /// </summary>
    [MapToComponent(typeof(IPositionComponent), "CurrentPosition")]
    public Position Position { get; set; } = Position.Empty;
    
    /// <summary>
    /// Team/Faction override for this spawn. If null, uses Definition value.
    /// </summary>
    public string? Team { get; set; }
    
    /// <summary>
    /// Controller ID override for this spawn. If null, uses Definition value.
    /// </summary>
    public string? ControllerId { get; set; }
}

