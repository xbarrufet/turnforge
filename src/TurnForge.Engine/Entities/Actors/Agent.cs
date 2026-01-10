using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Traits; // Base Actor
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public class Agent : Actor
{
    public static readonly Category AgentDefaultCategory = new("AgentCategory");

    // Direct properties (structural data)
    public TeamId Team { get; init; } = TeamId.Empty;
    public PlayerId Controller { get; init; } = PlayerId.Empty;

    // Constructor for Builder (with all properties)
    public Agent(
        EntityId id,
        string definitionId,
        string name,
        Category category,
        TeamId team,
        PlayerId controller,
        IBoardPositionId startPosition) : base(id, definitionId, name, category)
    {
        Team = team;
        Controller = controller;
        CurrentPosition = startPosition;
        _initializeComponents();
    }

    public Agent(
        EntityId id,
        string definitionId,
        string name,
        Category category) : base(id, definitionId, name, category)
    {
        _initializeComponents();

    }

    public Agent(
        EntityId id,
        string definitionId) : base(id, definitionId, definitionId, AgentDefaultCategory)
    {
        _initializeComponents();

    }
    public Agent(
        EntityId id,
        string definitionId,
        Category category) : base(id, definitionId, definitionId, category)
    {
        _initializeComponents();

    }

    private void _initializeComponents()
    {
        // Team is now a direct property, no need for MembershipTrait
    }

}