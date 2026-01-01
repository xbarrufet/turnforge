using NUnit.Framework;
using Moq;
using TurnForge.Engine.Commands.StartGame.Action;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions.Descriptors;
using System.Collections.Generic;
using TurnForge.Engine.Core.Action.Nodes;

namespace TurnForge.Engine.Tests.Commands.StartGame;

[TestFixture]
public class StartGameCommandTests
{
    private ActionOrchestrator _orchestrator = null!;
    private Mock<IBoardFactory> _boardFactoryMock = null!;
    private Mock<IEntityApplier> _entityApplierMock = null!;
    private Mock<IGameBoard> _gameBoardMock = null!;
    private Mock<IBoardDefinition> _boardDefinitionMock = null!;
    
    [SetUp]
    public void SetUp()
    {
        _orchestrator = new ActionOrchestrator();
        _boardFactoryMock = new Mock<IBoardFactory>();
        _entityApplierMock = new Mock<IEntityApplier>();
        _gameBoardMock = new Mock<IGameBoard>();
        _boardDefinitionMock = new Mock<IBoardDefinition>();
        
        _boardFactoryMock.Setup(f => f.CreateGameBoard(It.IsAny<IBoardDefinition>()))
            .Returns(_gameBoardMock.Object);
            
        // Setup default cloning for board if needed
        _gameBoardMock.Setup(b => b.Clone()).Returns(_gameBoardMock.Object);
    }
    
    private IAction CreateTestAction()
    {
         var processPlayer = new ProcessPlayerDataNode();
         var processBoard = new ProcessBoardDataNode(_boardFactoryMock.Object);
         var deployEntities = new DeployEntitiesNode(new NodeId("StartGame.DeployEntities"), _entityApplierMock.Object);
         var buildGame = new BuildGameNode();

         return ActionBuilder.Create("StartGame")
                 .AddNode(processPlayer)
                 .AddNode(processBoard)
                 .AddNode(deployEntities)
                 .AddNode(buildGame)
                 .Build();
    }
    
    private AddPlayerInput CreateAddPlayer(string name)
    {
        return new AddPlayerInput(PlayerId.From(Guid.NewGuid().ToString()), name, new List<AgentDeploymentInput>());
    }

    [Test]
    public void StartGameAction_FullFlow_CompletesSuccessfully()
    {
        // Arrange
        var workflow = CreateTestAction();
        var context = new StartGameActionContext(Guid.NewGuid(), GameState.Empty());
        
        // Act - Start workflow
        _orchestrator.StartAction(workflow, context);
        
        // Assert - Waiting for player input
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Suspended), "Should wait for player data");
        
        var workflowId = workflow.Id.Value;
        
        // Add first player
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player1"));
        
        // Still suspended
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Suspended));
        Assert.That(context.PlayerNames, Contains.Item("Player1"));
        
        // Add second player
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player2"));
        Assert.That(context.PlayerNames.Count, Is.EqualTo(2));
        
        // Confirm players
        _orchestrator.SubmitInput(workflowId, new ConfirmPlayersInput());
        
        // Waiting for map selection
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Suspended), "Should wait for map selection");
        Assert.That(context.PlayersConfirmed, Is.True);
        
        // Select map
        var missionData = new MissionData(
            MissionId: "mission-01",
            Name: "Test Mission",
            PlayerSpawnZones: new Dictionary<PlayerId, IBoardPosition>(),
            NamedLocations: new Dictionary<string, IBoardPosition>(),
            Objective: null
        );
        
        _orchestrator.SubmitInput(workflowId, new SelectMapInput("map-001", _boardDefinitionMock.Object, missionData));
        
        // Action should complete
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Completed), "Action should complete");
        Assert.That(context.MapId, Is.EqualTo("map-001"));
    }
    
    [Test]
    public void StartGameAction_ConfirmWithNoPlayers_StaysSuspended()
    {
        // Arrange
        var workflow = CreateTestAction();
        var context = new StartGameActionContext(Guid.NewGuid(), GameState.Empty());
        
        // Act
        _orchestrator.StartAction(workflow, context);
        _orchestrator.SubmitInput(workflow.Id.Value, new ConfirmPlayersInput());
        
        // Assert
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Suspended));
        Assert.That(context.PlayersConfirmed, Is.False, "Should not confirm with no players");
    }
    
    [Test]
    public void StartGameAction_DuplicatePlayerName_IsIgnored()
    {
        // Arrange
        var workflow = CreateTestAction();
        var context = new StartGameActionContext(Guid.NewGuid(), GameState.Empty());
        
        // Act
        _orchestrator.StartAction(workflow, context);
        var workflowId = workflow.Id.Value;
        
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player1"));
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player1")); // Duplicate
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player2"));
        
        // Assert
        Assert.That(context.PlayerNames.Count, Is.EqualTo(2));
        Assert.That(context.PlayerNames, Is.EquivalentTo(new[] { "Player1", "Player2" }));
    }
    
    [Test]
    public void StartGameAction_EmptyPlayerName_IsIgnored()
    {
        // Arrange
        var workflow = CreateTestAction();
        var context = new StartGameActionContext(Guid.NewGuid(), GameState.Empty());
        
        // Act
        _orchestrator.StartAction(workflow, context);
        var workflowId = workflow.Id.Value;
        
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer(""));
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("   ")); 
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player1"));
        
        // Assert
        Assert.That(context.PlayerNames.Count, Is.EqualTo(1));
        Assert.That(context.PlayerNames[0], Is.EqualTo("Player1"));
    }
}
