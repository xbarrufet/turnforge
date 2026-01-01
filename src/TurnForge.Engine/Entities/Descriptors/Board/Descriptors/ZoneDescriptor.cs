using System;
using System.Collections.Generic;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Traits.Interfaces;

using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;

namespace TurnForge.Engine.Entities.Board.Descriptors;

public record ZoneDescriptor : IGameEntityBuildDescriptor
{
    public string DefinitionId { get; init; }
    public List<IGameEntityComponent> ExtraComponents { get; init; }
    public List<IDataTrait> RequestedTraits { get; init; }
    
    public ZoneDescriptor(
        string definitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<IDataTrait>? requestedTraits = null)
    {
        DefinitionId = definitionId;
        ExtraComponents = extraComponents?.ToList() ?? new List<IGameEntityComponent>();
        RequestedTraits = requestedTraits?.ToList() ?? new List<IDataTrait>();
    }
}
