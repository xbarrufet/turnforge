using System.Text.Json;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Rules.BarelyAlive.Dto;
using TurnForge.Engine.Registration;
using TurnForge.Rules.BarelyAlive.Traits;

namespace TurnForge.Rules.BarelyAlive.Bootstrapper;

public sealed class PropDefinitionLoader(IDefinitionRegistry<PropTypeId, PropDefinition> registry)
{
    public void LoadFromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<PropDefinitionDto>(json)
                  ?? throw new InvalidOperationException("Invalid JSON");

        var behaviours = dto.Traits
            .Select(BarelyAliveTraitFactory.Create)
            .ToList();

        var definition = new PropDefinition(
            TypeId: new PropTypeId(dto.TypeId),
            MaxBaseMovement: 0,
            MaxActionPoints: 0,
            Behaviours: behaviours,
            MaxHealth: dto.MaxHealth ?? 1
        );
        registry.Register(definition.TypeId, definition);
        
    }
}
