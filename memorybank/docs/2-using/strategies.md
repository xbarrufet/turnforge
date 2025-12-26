# Implementing Logic with Strategies

Strategies encapsulate complex game logic, keeping Handlers clean and reusable.

## When to use a Strategy?
- **Handlers** should only manage the command workflow (validation -> routing).
- **Strategies** should contain the actual rules (calculations, pathfinding, cost checks).

## Example: Movement Strategy (Fast Track)

```csharp
public class BasicMoveStrategy : IActionStrategy<MoveCommand>
{
    public StrategyResult Execute(MoveCommand command, ActionContext context)
    {
        var agent = context.Query.GetAgent(command.AgentId);
        
        // 1. Validation
        if (!context.Board.IsValid(command.Destination))
            return StrategyResult.Failed("Invalid destination");
            
        // 2. Logic (Calculation)
        var path = context.Board.Pathfind(agent.Position, command.Destination);
        var cost = path.Length * 1;
        
        // 3. Result
        var decision = new MoveDecision(command.AgentId, command.Destination, cost);
        return StrategyResult.Completed(decision);
    }
}
```

## Interactive Strategies (Pipelines)

For complex actions requiring user input (e.g., dice rolls), use `PipelineStrategy<T>`.

### Structure
- **Nodes**: Small, stateless steps (e.g., `ValidateRange`, `RequestRoll`, `ApplyDamage`).
- **Context**: Data shared between nodes via `ActionContext.Variables`.
- **Suspension**: A node can return `NodeResult.Suspend()`, pausing execution until UI responds.

### Example Node
```csharp
public record RequestHitRollNode : IInteractionNode
{
    public NodeResult Execute(ActionContext context)
    {
        if (context.Variables.ContainsKey("HitRoll"))
            return NodeResult.Continue(); // Already rolled, move next
            
        // Suspend and ask UI to roll dice
        return NodeResult.Suspend(new InteractionRequest 
        { 
            Type = "DiceRoll", 
            Prompt = "Roll for Hit!" 
        });
    }
}
```

## Using Strategies in Handlers

Handlers are now generic and standardized (`ActionCommandHandler<T>`). They inject the strategy and handle the `StrategyResult` automatically, managing the `InteractionRegistry` if suspension occurs.

```csharp
// No need to write manual handlers for standard actions!
// process logic is encapsulated in the Strategy.
services.RegisterSingleton<IActionStrategy<MoveCommand>>(new BasicMoveStrategy());
```
