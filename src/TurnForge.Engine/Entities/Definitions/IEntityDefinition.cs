namespace TurnForge.Engine.Entities;

public interface IEntityDefinition
{
    public string DefinitionId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
}