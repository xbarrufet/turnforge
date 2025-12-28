using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions.Actors;
using BarelyAlive.Rules.Events;

namespace BarelyAlive.Rules.Decisions;

/// <summary>
/// Decision to destroy/remove an entity from the game.
/// </summary>
public class DestroyEntityDecision : IDecision
{
    public EntityId Target { get; }
    public EntityId? Killer { get; }
    public string Cause { get; }
    
    public DecisionTiming Timing => DecisionTiming.Immediate;
    public string OriginId => "BarelyAlive.DestroyEntity";
    
    public DestroyEntityDecision(EntityId target, EntityId? killer, string cause)
    {
        Target = target;
        Killer = killer;
        Cause = cause;
    }
    
    public GameState Apply(GameState state)
    {
        // Check if it's an agent
        if (state.Agents.ContainsKey(Target))
        {
            // Remove from agents dictionary
            var newAgents = state.Agents.Remove(Target);
            return new GameStateBuilder(state).WithAgents(newAgents.Values).Build();
        }
        
        // Check if it's a prop
        if (state.Props.ContainsKey(Target))
        {
            var newProps = state.Props.Remove(Target);
            return new GameStateBuilder(state).WithProps(newProps.Values).Build();
        }
        
        return state;
    }
}

// Simple builder helper
internal class GameStateBuilder
{
    private readonly GameState _base;
    private IEnumerable<Agent>? _agents;
    private IEnumerable<Prop>? _props;
    
    public GameStateBuilder(GameState baseState) => _base = baseState;
    
    public GameStateBuilder WithAgents(IEnumerable<Agent> agents)
    {
        _agents = agents;
        return this;
    }
    
    public GameStateBuilder WithProps(IEnumerable<Prop> props)
    {
        _props = props;
        return this;
    }
    
    public GameState Build()
    {
        var result = _base;
        if (_agents != null)
            result = result.WithAgents(_agents, replaceAll: true);
        if (_props != null)
            result = result.WithProps(_props, replaceAll: true);
        return result;
    }
}
