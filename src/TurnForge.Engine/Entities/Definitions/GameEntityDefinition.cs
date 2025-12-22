using TurnForge.Engine.Entities.Behaviours.Interfaces;

namespace TurnForge.Engine.Entities;

public abstract class GameEntityDefinition
{
    public string DefinitionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

}