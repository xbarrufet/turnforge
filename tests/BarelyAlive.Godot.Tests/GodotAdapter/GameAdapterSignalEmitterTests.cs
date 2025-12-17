using NUnit.Framework;
using Godot;
using Godot.Collections;
using TurnForge.Engine.Core;
using BarelyAlive.Godot.TurnForge.GodotAdapter;

namespace BarelyAlive.Godot.Tests.GodotAdapter;

[TestFixture]
public class GameAdapterSignalEmitterTests
{
    private GameAdapterSignalEmitter _emitter;
    private List<(string commandName, Dictionary payload)> _emittedSignals;

    [SetUp]
    public void Setup()
    {
        _emitter = new GameAdapterSignalEmitter();
        _emittedSignals = new List<(string, Dictionary)>();

        // Connect to the generic EngineResponse signal
        _emitter.Connect(nameof(GameAdapterSignalEmitter.EngineResponse),
            new Callable(this, nameof(OnEngineResponse)));
    }

    [TearDown]
    public void TearDown()
    {
        _emitter.Disconnect(nameof(GameAdapterSignalEmitter.EngineResponse),
            new Callable(this, nameof(OnEngineResponse)));
    }

    [Test]
    public void Emit_GameStartCommandResult_should_emit_signal_with_correct_structure()
    {
        // Arrange
        var result = new CommandResponseEnvelope(
            CommandName: "GameStartCommand",
            Payload: new { message = "Game started successfully" }
        );

        // Act
        _emitter.OnEngineEvent(result);

        // Assert
        Assert.That(_emittedSignals.Count, Is.GreaterThan(0), "Should emit at least one signal");
        var (commandName, payload) = _emittedSignals[0];
        Assert.That(commandName, Is.EqualTo("GameStartCommand"));
        Assert.That(payload, Is.Not.Null);
    }

    [Test]
    public void Emit_should_convert_CLR_objects_to_Godot_Collections()
    {
        // Arrange
        var objWithCollection = new
        {
            items = new[] { 1, 2, 3 },
            name = "test"
        };

        var result = new CommandResponseEnvelope(
            CommandName: "TestCommand",
            Payload: objWithCollection
        );

        // Act
        _emitter.OnEngineEvent(result);

        // Assert
        Assert.That(_emittedSignals.Count, Is.GreaterThan(0));
        var (_, payload) = _emittedSignals[0];
        Assert.That(payload, Is.InstanceOf<Dictionary>());
    }

    [Test]
    public void Emit_should_handle_null_payload()
    {
        // Arrange
        var result = new CommandResponseEnvelope(
            CommandName: "NullPayloadCommand",
            Payload: null
        );

        // Act
        _emitter.OnEngineEvent(result);

        // Assert
        Assert.That(_emittedSignals.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Emit_should_handle_nested_objects()
    {
        // Arrange
        var nestedObj = new
        {
            outer = new
            {
                inner = "value",
                count = 42
            },
            list = new[] { "a", "b", "c" }
        };

        var result = new CommandResponseEnvelope(
            CommandName: "NestedCommand",
            Payload: nestedObj
        );

        // Act
        _emitter.OnEngineEvent(result);

        // Assert
        Assert.That(_emittedSignals.Count, Is.GreaterThan(0));
        var (_, payload) = _emittedSignals[0];
        Assert.That(payload, Is.InstanceOf<Dictionary>());
    }

    // Signal handler
    public void OnEngineResponse(string commandName, Dictionary payload)
    {
        _emittedSignals.Add((commandName, payload));
    }
}
