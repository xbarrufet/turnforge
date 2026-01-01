using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Entities.Descriptors.Interfaces;

public interface IGameEntityBuildDescriptor
{
    public    string DefinitionId { get; }
    List<IGameEntityComponent> ExtraComponents { get; }
    List<TurnForge.Engine.Traits.Interfaces.IDataTrait> RequestedTraits { get; }
}