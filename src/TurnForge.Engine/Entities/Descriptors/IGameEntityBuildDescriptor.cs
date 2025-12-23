using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Entities.Descriptors;

public interface IGameEntityBuildDescriptor
{
    public string DefinitionID { get; set; }
    public List<IGameEntityComponent> ExtraComponents { get; }
}