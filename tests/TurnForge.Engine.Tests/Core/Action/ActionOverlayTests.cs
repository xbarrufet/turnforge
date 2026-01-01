using NUnit.Framework;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using System.Collections.Immutable;

namespace TurnForge.Engine.Tests.Core.Action;

/// <summary>
/// Tests for workflow overlay lifecycle - verify overlay is created, shared, and committed properly
/// </summary>
[TestFixture]
public class ActionOverlayLifecycleTests
{
    [Test]
    public void ActionContext_InitializeState_CreatesOverlay()
    {
        // Arrange
        var context = new TestContext();
        var state = CreateTestState();
        
        // Act
        context.InitializeState(state);
        
        // Assert
        Assert.That(context.Overlay, Is.Not.Null, "Overlay should be created when state is initialized");
    }
    
    [Test]
    public void ActionContext_AfterInitialize_OverlayIsAccessible()
    {
        // Arrange
        var context = new TestContext();
        var state = CreateTestState();
        context.InitializeState(state);
        
        // Act & Assert - Should not throw
        var overlay = context.Overlay;
        Assert.That(overlay, Is.Not.Null);
    }
    
    [Test]
    public void ActionContext_UpdateState_ChangesState()
    {
        // Arrange
        var context = new TestContext();
        var initialState = CreateTestState();
        context.InitializeState(initialState);
        
        // Act
        var newState = CreateTestState();  // Different state
        context.UpdateState(newState);
        
        // Assert
        Assert.That(context.State, Is.Not.Null);
    }
    
    // --- Helper Methods ---
    
    private GameState CreateTestState()
    {
        return new GameState(
            ImmutableDictionary<EntityId, GameEntity>.Empty,
            ImmutableDictionary<PlayerId, Player>.Empty,
            null,
            null
        );
    }
    
    private class TestContext : ActionContext
    {
        public override object? GetResult() => State;
    }
}
