using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Game;
using TurnForge.Engine.APIs.Interfaces;

namespace BarelyAlive.Rules.Apis.Handlers;

public class GetSurvivorsHandler
{
    private readonly IGameCatalogApi _catalog;

    public GetSurvivorsHandler(IGameCatalogApi catalog)
    {
        _catalog = catalog;
    }

    public List<SurvivorDefinition> Handle(GetRegisteredSurvivorsQuery query)
    {
        var definitions = _catalog.GetAgentsByCategory("Survivor");
        return definitions
            .Select(MapToSurvivor)
            .ToList();
    }

    private SurvivorDefinition MapToSurvivor(TurnForge.Engine.Entities.GameEntityDefinition def)
    {
        // Simple projection for now. 
        // In the future, we might look up "Description" or "Sprite" from another source using the ID.
        return new SurvivorDefinition(
            Id: def.AgentName,
            Name: def.AgentName, // Use ID as Name for now
            Description: "A survivor ready for action.",
            MaxHealth: def.MaxHealth,
            MaxMovement: def.MaxMovement
        );
    }
}
