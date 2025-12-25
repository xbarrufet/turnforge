# Sending Commands

Commands represent user or AI intent. To add new gameplay actions, you implement the **Command-Handler-Applier** pattern.

## 1. Define the Command

Commands must be immutable records implementing `ICommand`.

```csharp
public sealed record AttackCommand(
    string AttackerId, 
    string TargetId
) : ICommand
{
    public Type CommandType => typeof(AttackCommand);
}
```

## 2. Create the Handler

The Handler validates business logic but **does not mutate state**. It returns `Decisions`.

```csharp
public class AttackHandler : ICommandHandler<AttackCommand>
{
    public CommandResult Handle(AttackCommand command)
    {
        // 1. Validation Logic
        if (!IsValidTarget(command.TargetId))
            return CommandResult.Failed("Invalid target");

        // 2. Generate Decision
        var decision = new AttackDecision(command.AttackerId, command.TargetId);
        
        return CommandResult.Ok([decision]);
    }
}
```

## 3. Create the Decision

Decisions act as instructions for the Orchestrator.

```csharp
public record AttackDecision(string AttackerId, string TargetId) : IDecision
{
    public DecisionTiming Timing { get; init; } = DecisionTiming.Immediate;
}
```

## 4. Create the Applier

The Applier executes the decision and **mutates state**.

```csharp
public class AttackApplier : IApplier<AttackDecision>
{
    public ApplierResponse Apply(AttackDecision decision, GameState state)
    {
        var target = state.Agents[decision.TargetId];
        var newHealth = target.Health - 10;
        
        // Mutate State (Create new instances)
        var updatedTarget = target.WithHealth(newHealth);
        var newState = state.WithAgent(updatedTarget);
        
        // Generate Effect
        var effect = new AgentDamagedEffect(decision.TargetId, 10);
        
        return new ApplierResponse(newState, [effect]);
    }
}
```

## 5. Register Everything

```csharp
// Register Handler (typically via DI)
services.AddSingleton<ICommandHandler<AttackCommand>, AttackHandler>();

// Register Applier (in Orchestrator)
orchestrator.RegisterApplier(new AttackApplier());

// Allow Command in FSM Node
public class MyTurnNode : LeafNode
{
    public MyTurnNode()
    {
        AddAllowedCommand<AttackCommand>();
    }
}
```
