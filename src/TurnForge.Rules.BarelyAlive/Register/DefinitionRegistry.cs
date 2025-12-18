using TurnForge.Engine.Registration;

namespace TurnForge.Rules.BarelyAlive.Register;

public sealed class DefinitionRegistry<TTypeId, TDefinition>
    : IDefinitionRegistry<TTypeId, TDefinition>
{
    private readonly Dictionary<TTypeId, TDefinition> _definitions = new();

    public void Register(TTypeId id, TDefinition definition)
        => _definitions[id] = definition;

    public TDefinition Get(TTypeId id)
        => _definitions[id]
           ?? throw new KeyNotFoundException($"Definition {id} not found");

    public bool TryGet(TTypeId id, out TDefinition def)
        => _definitions.TryGetValue(id, out def);
}
