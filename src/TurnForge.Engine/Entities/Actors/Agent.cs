using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public  class Agent : Actor
{
    public Agent(
        EntityId id,
        string definitionId,
        string name,
        string category) : base(id, name, category, definitionId)
    {
    }
}