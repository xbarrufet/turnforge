using TurnForge.Engine.Entities.TraitsComponents.Interfaces;

namespace TurnForge.Engine.Entities.Descriptors;

public interface IGameEntityBuildDescriptor
{
    string DefinitionId { get; }
    string Name { get; }
    List<IGameEntityComponent> ExtraComponents { get; }
    void AddExtraComponent(IGameEntityComponent component);
    List<ITrait> DefinitionTraitValues { get; }
    bool TryGetTraitValue(Type traitType, out ITrait? trait);
    bool TryGetExtraComponent(Type componentType, out IGameEntityComponent? component);
    void AddDefinitionTraitValue(ITrait trait);
}