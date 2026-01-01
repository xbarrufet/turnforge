using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Entities.Actors; // Base Actor
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public  class Prop(
    EntityId id,
    string definitionId,
    string name,
    string category) : Actor(id, name, category, definitionId)
{
}