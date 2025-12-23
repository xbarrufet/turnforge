using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Definitions;
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

    public List<BarelyAlive.Rules.Apis.Messaging.SurvivorDefinition> Handle(GetRegisteredSurvivorsQuery query)
    {
        // Get all definitions and filter by Survivor category
        // Note: Using fully qualified names to avoid ambiguity between DTO and Domain Definition
        var allDefs = _catalog.GetAllDefinitions<BarelyAlive.Rules.Core.Domain.Definitions.SurvivorDefinition>();
        
        return allDefs
            .Select(MapToSurvivorDto)
            .ToList();
    }

    private BarelyAlive.Rules.Apis.Messaging.SurvivorDefinition MapToSurvivorDto(BarelyAlive.Rules.Core.Domain.Definitions.SurvivorDefinition def)
    {
        return new BarelyAlive.Rules.Apis.Messaging.SurvivorDefinition(
            Id: def.DefinitionId,
            Name: def.Name, 
            Description: "A survivor ready for action.",
            MaxHealth: def.MaxHealth,
            MaxMovement: 3 // Default movement for now as it was removed from base definition
        );
    }
    }

