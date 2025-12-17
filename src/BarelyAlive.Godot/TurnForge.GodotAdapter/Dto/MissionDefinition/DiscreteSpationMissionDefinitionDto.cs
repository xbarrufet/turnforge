using System.Collections.Generic;

namespace TurnForge.GodotAdapter.Dto.MissionDefinition;

public readonly struct DiscreteSpatialMissionDefinitionDto(DiscreteSpatialDefinitionDto spatial, IReadOnlyList<ActorDefinitionDto> actors)
{
    
    public DiscreteSpatialDefinitionDto SpatialDefinition { get; init; }= spatial;
    public IReadOnlyList<ActorDefinitionDto> Actors { get; init; } = actors;
    
}
