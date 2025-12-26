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

## DiceThrowService

Service for rolling dice with support for modifiers and pass/fail checks.

**Namespace:** `TurnForge.Engine.Services.Dice`

### Basic Usage

```csharp
// Create service (inject Random for deterministic testing)
var service = new DiceThrowService(new Random(42));

// Simple roll
var damage = service.Roll("2D6+3");
Console.WriteLine($"Damage: {damage.Total}"); // e.g., 11

// Check pass/fail
var dangerCheck = service.Roll("1D6", "4+");
if (dangerCheck.Pass == true) { /* Safe */ }

// Fluent comparisons
if (damage.IsHigherOrEqualThan(10)) { /* Critical hit */ }
```

### Dice Notation

| Notation | Description |
|----------|-------------|
| `2D6` | Roll 2 six-sided dice |
| `3D6+5` | Roll 3D6 and add 5 |
| `4D6kh3` | Roll 4D6, keep 3 highest |
| `3D6kl2` | Roll 3D6, keep 2 lowest |
| `2D6r1` | Roll 2D6, reroll 1s once |
| `4D6kh3+2` | Combined: keep highest + modifier |

### Fluent Builders

```csharp
// Build with fluent API
var advantage = DiceThrowType.Parse("2D20").KeepHighest(1);
var rerollOnes = DiceThrowType.Parse("3D6").Reroll(1, MaxTimes: 2);

var result = service.Roll(advantage);
```

### History Tracking

```csharp
// Enable history for debugging/UI
var result = service.Roll("4D6kh3", trackHistory: true);

foreach (var entry in result.History!)
{
    Console.WriteLine($"{entry.OriginalValue} → {entry.Reason}");
}
// Output: 2 → Dropped, 4 → Kept, 5 → Kept, 6 → Kept
```

### In Strategies

```csharp
public class DangerMoveStrategy : IActionStrategy
{
    private readonly IDiceThrowService _dice;
    
    public IEnumerable<IDecision> Execute(IActionCommand command, ActionContext ctx)
    {
        // Check if move through dangerous terrain passes
        var check = _dice.Roll("1D6", "4+");
        
        if (check.Pass != true)
        {
            // Failed - apply damage
            var damage = _dice.Roll("1D3");
            yield return new ActionDecision(entity.Health.WithDamage(damage.Total));
        }
    }
}
```

## Future Services

- **PathfindingService**: For calculating paths on the board.

