// csharp
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public  class Prop(
    EntityId id,
    string definitionId,
    string name,
    string category) : Actor(id, name, category, definitionId)
{
}