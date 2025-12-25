# Implementing Logic with Strategies

Strategies encapsulate complex game logic, keeping Handlers clean and reusable.

## When to use a Strategy?
- **Handlers** should only manage the command workflow (validation -> routing).
- **Strategies** should contain the actual rules (calculations, pathfinding, cost checks).

## Example: Movement Strategy

```csharp
public interface IMoveStrategy
{
    MovementResult CalculateMovement(Agent agent, Position target);
}

public class GridMoveStrategy : IMoveStrategy
{
    public MovementResult CalculateMovement(Agent agent, Position target)
    {
        var distance = Math.Abs(agent.Position.X - target.X) + Math.Abs(agent.Position.Y - target.Y);
        var cost = distance * 1; // 1 AP per tile
        
        if (agent.CurrentAP < cost)
            return MovementResult.Fail("Not enough AP");
            
        return MovementResult.Success(cost);
    }
}
```

## Using Strategies in Handlers

Inject the strategy into your handler.

```csharp
public class MoveHandler : ICommandHandler<MoveCommand>
{
    private readonly IMoveStrategy _strategy;
    
    public MoveHandler(IMoveStrategy strategy)
    {
        _strategy = strategy;
    }

    public CommandResult Handle(MoveCommand command)
    {
        var agent = _repository.LoadAgent(command.AgentId);
        
        // Delegate logic to strategy
        var result = _strategy.CalculateMovement(agent, command.Target);
        
        if (!result.Success)
            return CommandResult.Failed(result.Error);
            
        var decision = new MoveDecision(agent.Id, command.Target, result.Cost);
        return CommandResult.Ok([decision]);
    }
}
```

## Strategy Benefits
1. **Testable**: Unit test logic without mocking the whole engine.
2. **Swappable**: Change rules (e.g., `FlightMoveStrategy` vs `WalkMoveStrategy`) without changing command flow.
