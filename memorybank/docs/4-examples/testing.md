# Testing Patterns

TurnForge is designed for testability. Key patterns for unit and integration testing.

## FSM Tests

Test flow logic by setting up a minimal FSM tree.

```csharp
[Test]
public void MoveForward_TransitionsCorrectly()
{
    // 1. Setup minimal FSM
    var builder = new GameFlowBuilder();
    var root = builder.AddRoot<object>("Root", r => {
        r.AddLeaf<SpyNode>("Phase1");
        r.AddLeaf<SpyNode>("Phase2");
    }).Build();
    
    var controller = new FsmController(root, root.Id);
    var repository = new InMemoryGameRepository();
    
    // 2. Sync initial state
    var state = repository.LoadGameState()
        .WithCurrentStateId(controller.CurrentStateId);
    repository.SaveGameState(state);
    
    // 3. Act: Trigger transition
    var result = controller.MoveForwardRequest(state);
    repository.SaveGameState(result.State);
    
    // 4. Assert
    Assert.That(result.State.CurrentStateId, Is.EqualTo("Root/Phase2"));
}
```

## Applier Tests

Test state mutations in isolation.

```csharp
[Test]
public void SpawnApplier_CreatesEntity()
{
    // 1. Prepare data
    var descriptor = new SurvivorDescriptor("Survivors.Mike") { Position = new Position(1, 1) };
    var decision = new SpawnDecision<SurvivorDescriptor>(descriptor);
    var applier = new AgentSpawnApplier(new MockFactory());
    
    // 2. Act
    var response = applier.Apply(decision, GameState.Empty());
    
    // 3. Assert
    Assert.That(response.GameState.Agents, Has.Count.EqualTo(1));
    Assert.That(response.GameEffects[0], Is.InstanceOf<EntitySpawnedEffect>());
}
```

## Common Pitfalls

| Issue | Symptom | Solution |
|-------|---------|----------|
| **Not syncing repository after FSM init** | `CurrentStateId` is null | Always sync after `FsmController` creation |
| **Expecting BranchNode start state** | Tests fail unexpectedly | Account for auto-navigation to first leaf |
| **State mutation in handler** | Inconsistent state | Handlers return decisions only, never mutate |
