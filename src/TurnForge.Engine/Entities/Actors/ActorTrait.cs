using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors;

public readonly struct ActorTrait(string name, IReadOnlyDictionary<string, string> attributes) : IActorTrait
{
    public string TraitName { get; init; } = name;
    public IReadOnlyDictionary<string, string> Attributes { get; init; } = attributes;

    public ActorTrait(ActorTraitDefinition definition) :
        this(definition.Name,
            definition.Attributes)
    {
        
    }

}