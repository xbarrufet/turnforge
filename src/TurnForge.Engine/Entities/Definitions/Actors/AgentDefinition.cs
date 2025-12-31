using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Actors;

/// <summary>
/// Base definition for Agent entities.
/// Agents are located actors that can execute commands from a Player.
/// </summary>
public abstract class AgentDefinition : ActorDefinition
{
    /// <summary>
    /// The PlayerId of the player that owns/controls this agent.
    /// </summary>
    public PlayerId OwnerId { get; }

    protected AgentDefinition(string definitionId, PlayerId ownerId) 
        : base(definitionId, "Agent")
    {
        OwnerId = ownerId;
        AddTrait(new ActionableByPlayerTrait(ownerId));
    }


    protected AgentDefinition(string definitionId, string category, PlayerId ownerId) 
        : base(definitionId, category)
    {
        OwnerId = ownerId;
        AddTrait(new ActionableByPlayerTrait(ownerId));
    }
}