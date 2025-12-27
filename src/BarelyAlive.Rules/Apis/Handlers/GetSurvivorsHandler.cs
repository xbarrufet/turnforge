using BarelyAlive.Rules.Core.Domain.Entities; 
using TurnForge.Engine.APIs.Interfaces;
using BarelyAlive.Rules.Apis.Messaging;
using System.Linq; // Ensure LINQ is available

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
        var allDefs = _catalog.GetAllDefinitions<BarelyAlive.Rules.Core.Domain.Entities.SurvivorDefinition>();
        
        return allDefs
            .Select(MapToSurvivorDto)
            .ToList();
    }

    private BarelyAlive.Rules.Apis.Messaging.SurvivorDefinition MapToSurvivorDto(BarelyAlive.Rules.Core.Domain.Entities.SurvivorDefinition def)
    {
        // Extract Name from IdentityTrait
        var identity = def.Traits.OfType<TurnForge.Engine.Traits.Standard.IdentityTrait>().FirstOrDefault();
        var name = identity?.Name ?? def.DefinitionId;

        return new BarelyAlive.Rules.Apis.Messaging.SurvivorDefinition(
            Id: def.DefinitionId,
            Name: name, 
            Description: "A survivor ready for action.",
            MaxHealth: def.MaxHealth,
            MaxMovement: 3 
        );
    }
    }

