using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Actors;

/// <summary>
/// Base definition for Agent entities.
/// Agents are located actors that can execute commands from a Player.
/// </summary>
public abstract class AgentDefinition : ActorDefinition
{

    protected AgentDefinition(string definitionId)
        : base(definitionId, Agent.AgentDefaultCategory)
    {
        // MembershipTrait removed - Team is now a direct property on Agent
    }


    protected AgentDefinition(string definitionId, Category category)
        : base(definitionId, category)
    {
        // MembershipTrait removed - Team is now a direct property on Agent
    }
}