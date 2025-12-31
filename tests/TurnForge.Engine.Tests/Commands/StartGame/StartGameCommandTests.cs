using NUnit.Framework;
using Moq;
using TurnForge.Engine.Commands.StartGame.Workflow;
using TurnForge.Engine.Commands.StartGame.Workflow.Inputs;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Builders;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions.Descriptors;
using System.Collections.Generic;
using TurnForge.Engine.Core.Workflow.Nodes;

namespace TurnForge.Engine.Tests.Commands.StartGame;

[TestFixture]
public class StartGameCommandTests
{
    private WorkflowOrchestrator _orchestrator = null!;
    private Mock<IBoardFactory> _boardFactoryMock = null!;
    private Mock<IEntityApplier> _entityApplierMock = null!;
    private Mock<IGameBoard> _gameBoardMock = null!;
    private Mock<IBoardDefinition> _boardDefinitionMock = null!;
    
    [SetUp]
    public void SetUp()
    {
        _orchestrator = new WorkflowOrchestrator();
        _boardFactoryMock = new Mock<IBoardFactory>();
        _entityApplierMock = new Mock<IEntityApplier>();
        _gameBoardMock = new Mock<IGameBoard>();
        _boardDefinitionMock = new Mock<IBoardDefinition>();
        
        _boardFactoryMock.Setup(f => f.CreateGameBoard(It.IsAny<IBoardDefinition>()))
            .Returns(_gameBoardMock.Object);
            
        // Setup default cloning for board if needed
        _gameBoardMock.Setup(b => b.Clone()).Returns(_gameBoardMock.Object);
    }
    
    private IWorkflow CreateTestWorkflow()
    {
         var processPlayer = new ProcessPlayerDataNode();
         var processBoard = new ProcessBoardDataNode(_boardFactoryMock.Object);
         var deployEntities = new DeployEntitiesNode(new NodeId("StartGame.DeployEntities"), _entityApplierMock.Object);
         var buildGame = new BuildGameNode();

         return WorkflowBuilder.Create("StartGame")
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
    public void StartGameWorkflow_FullFlow_CompletesSuccessfully()
    {
        // Arrange
        var workflow = CreateTestWorkflow();
        var context = new StartGameWorkflowContext(Guid.NewGuid(), GameState.Empty());
        
        // Act - Start workflow
        _orchestrator.StartWorkflow(workflow, context);
        
        // Assert - Waiting for player input
        Assert.That(context.Status, Is.EqualTo(WorkflowStatus.Suspended), "Should wait for player data");
        
        var workflowId = workflow.Id.Value;
        
        // Add first player
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player1"));
        
        // Still suspended
        Assert.That(context.Status, Is.EqualTo(WorkflowStatus.Suspended));
        Assert.That(context.PlayerNames, Contains.Item("Player1"));
        
        // Add second player
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player2"));
        Assert.That(context.PlayerNames.Count, Is.EqualTo(2));
        
        // Confirm players
        _orchestrator.SubmitInput(workflowId, new ConfirmPlayersInput());
        
        // Waiting for map selection
        Assert.That(context.Status, Is.EqualTo(WorkflowStatus.Suspended), "Should wait for map selection");
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
        
        // Workflow should complete
        Assert.That(context.Status, Is.EqualTo(WorkflowStatus.Completed), "Workflow should complete");
        Assert.That(context.MapId, Is.EqualTo("map-001"));
    }
    
    [Test]
    public void StartGameWorkflow_ConfirmWithNoPlayers_StaysSuspended()
    {
        // Arrange
        var workflow = CreateTestWorkflow();
        var context = new StartGameWorkflowContext(Guid.NewGuid(), GameState.Empty());
        
        // Act
        _orchestrator.StartWorkflow(workflow, context);
        _orchestrator.SubmitInput(workflow.Id.Value, new ConfirmPlayersInput());
        
        // Assert
        Assert.That(context.Status, Is.EqualTo(WorkflowStatus.Suspended));
        Assert.That(context.PlayersConfirmed, Is.False, "Should not confirm with no players");
    }
    
    [Test]
    public void StartGameWorkflow_DuplicatePlayerName_IsIgnored()
    {
        // Arrange
        var workflow = CreateTestWorkflow();
        var context = new StartGameWorkflowContext(Guid.NewGuid(), GameState.Empty());
        
        // Act
        _orchestrator.StartWorkflow(workflow, context);
        var workflowId = workflow.Id.Value;
        
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player1"));
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player1")); // Duplicate
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player2"));
        
        // Assert
        Assert.That(context.PlayerNames.Count, Is.EqualTo(2));
        Assert.That(context.PlayerNames, Is.EquivalentTo(new[] { "Player1", "Player2" }));
    }
    
    [Test]
    public void StartGameWorkflow_EmptyPlayerName_IsIgnored()
    {
        // Arrange
        var workflow = CreateTestWorkflow();
        var context = new StartGameWorkflowContext(Guid.NewGuid(), GameState.Empty());
        
        // Act
        _orchestrator.StartWorkflow(workflow, context);
        var workflowId = workflow.Id.Value;
        
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer(""));
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("   ")); 
        _orchestrator.SubmitInput(workflowId, CreateAddPlayer("Player1"));
        
        // Assert
        Assert.That(context.PlayerNames.Count, Is.EqualTo(1));
        Assert.That(context.PlayerNames[0], Is.EqualTo("Player1"));
    }
}
