# Engine Services

Services provide reusable functionality to Strategies and Handlers.

## GameStateQueryService

Use this service to query the game state without direct access to the `GameState` object (useful in Strategies).

```csharp
public interface IGameStateQuery
{
    Agent? GetAgent(string agentId);
    IEnumerable<Agent> GetAgentsAt(Position position);
    bool IsPositionOccupied(Position position);
}
```

**Usage:**

```csharp
public class MyStrategy
{
    private readonly IGameStateQuery _query;
    
    public void CheckTarget(string targetId)
    {
        var target = _query.GetAgent(targetId);
        if (target != null) { ... }
    }
}
```

## Board Queries

Access the `Board` entity via the `IGameStateQuery` or directly in handlers to check spatial rules.

- `board.IsValid(pos)`
- `board.Distance(from, to)`
- `board.GetNeighbors(pos)`

## Future Services

- **DiceThrowService**: For RNG and dice roll validation.
- **PathfindingService**: For calculating paths on the board.
