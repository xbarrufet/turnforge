namespace TurnForge.Engine.Entities.Descriptors;

public class GameEntityBuildDescriptor(string definitionId):IGameEntityBuildDescriptor
{
    public string DefinitionID { get; set; } = definitionId;
}