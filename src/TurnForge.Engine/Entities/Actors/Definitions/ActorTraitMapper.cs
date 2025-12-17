using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public static class ActorTraitConverter
{
    public static IReadOnlyList<IActorTrait>? ToTraits(
        IReadOnlyDictionary<string, IReadOnlyList<ActorTraitDefinition>>? traitGroups)
    {
        if (traitGroups == null || traitGroups.Count == 0)
            return null;

        var list = new List<IActorTrait>(traitGroups.Sum(g => g.Value.Count));

        foreach (var (traitType, defs) in traitGroups)
        {
            foreach (var def in defs)
            {
                // Create a runtime trait from the definition.
                // If you have concrete trait types, dispatch by `traitType` here.
                list.Add(new ActorTrait(traitType, def.Attributes));
            }
        }

        return list;
    }
}

