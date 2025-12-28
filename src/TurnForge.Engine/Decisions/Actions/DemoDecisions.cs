using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions;

namespace TurnForge.Engine.Decisions.Actions;

public record MoveDecision(string EntityId, Position Target, string OriginId = "System") : IDecision
{
    public DecisionTiming Timing => DecisionTiming.Immediate;
    public string Description => $"Move {EntityId} to {Target}";

    public Definitions.GameState Apply(Definitions.GameState state)
    {
         // Assuming Agents for demo
         var id = new TurnForge.Engine.ValueObjects.EntityId(System.Guid.Parse(EntityId));
         if (state.Agents.TryGetValue(id, out var agent))
         {
             var clone = (Definitions.Actors.Agent)agent.Clone();
             clone.ReplaceComponent(new TurnForge.Engine.Components.BasePositionComponent(Target));
             return state.WithAgent(clone);
         }
         return state;
    }
}

public record DamageDecision(string EntityId, int Amount, string OriginId = "System") : IDecision
{
    public DecisionTiming Timing => DecisionTiming.Immediate;
    public string Description => $"Apply {Amount} damage to {EntityId}";

    public Definitions.GameState Apply(Definitions.GameState state)
    {
         // Placeholder: Real Game would have HealthComponent logic here.
         // Since we don't have explicit HealthComponent in namespace yet or logic is generic, 
         // we just return state to satisfy interface.
         return state;
    }
}
