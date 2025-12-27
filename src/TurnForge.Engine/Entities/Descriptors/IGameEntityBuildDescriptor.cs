using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Definitions.Descriptors;

public interface IGameEntityBuildDescriptor
{
    public    string DefinitionId { get; }
    List<IGameEntityComponent> ExtraComponents { get; }
    List<TurnForge.Engine.Traits.Interfaces.IBaseTrait> RequestedTraits { get; }
}