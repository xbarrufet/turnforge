using NUnit.Framework;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Definitions.Actors; // For Agent
using TurnForge.Engine.Commands.StartGame;
using TurnForge.Engine.Entities.Actors; // For Agent
using TurnForge.Engine.Commands.StartGame.Action;
using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Factory;
using Parchis.Rules.Board;
using Parchis.Rules.Entities;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions.Board;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces; // For IGameEntityBuildDescriptor
using System;

namespace ParchisLudo.Tests;

[TestFixture]
public class ParchisSpawnIntegrationTests
{
    private ActionOrchestrator _orchestrator;
    private InMemoryGameCatalog _catalog;
    private IBoardFactory _boardFactory; 
    private Mock<IEntityApplier> _entityApplierMock;

    [SetUp]
    public void Setup()
    {
        // 1. Setup Infrastructure
        _catalog = new InMemoryGameCatalog();
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<ActionOrchestrator>>();
        
        // Setup Board Factory Mock
        var boardFactoryMock = new Mock<IBoardFactory>();
        boardFactoryMock.Setup(x => x.CreateGameBoard(It.IsAny<IBoardDefinition>()))
            .Returns((IBoardDefinition def) => {
                 // Create Real Topology from Parchis Rules
                 var parchisDef = ParchisBoardFactory.CreateDescriptor();
                 
                 // Use Edges from the created definition
                 var edges = parchisDef.Edges
                    .Select(c => (c.positionFrom, c.positionTo))
                    .ToList();
                 
                 // Instantiate the factory
                 var topologyFactory = new BoardTopologyFactory();
                 var topology = topologyFactory.CreateDiscreteTopology(edges);
                 
                 // Create GameBoard
                 var spatial = new TurnForge.Engine.Entities.Board.SpatialIndex();
                 return new GameBoard(EntityId.New(), BoardKind.Discrete, topology, spatial);
            });
        _boardFactory = boardFactoryMock.Object;

        // Mock Entity Applier - it requires complex DI setup
        _entityApplierMock = new Mock<IEntityApplier>();
        _entityApplierMock.Setup(x => x.Apply(It.IsAny<IGameEntityBuildDescriptor>(), It.IsAny<IBoardPosition>()))
            .Returns((IGameEntityBuildDescriptor desc, IBoardPosition pos) => {
                var entity = new Agent(
                    EntityId.New(),
                    desc.DefinitionId,
                    desc.DefinitionId,
                    "Pawn"
                );
                return new SpawnEntityOperation(entity.Id, entity, pos);
            });
        
        _orchestrator = new ActionOrchestrator(loggerMock.Object);
    }

    [Test]
    public void Parchis_StartGame_SpawnsPawnsAtHomeBases()
    {
        // ARRANGE
        // 1. Register Definitions
        var playerTemplateId = PlayerId.From("template");
        var redPawnDef = new ParchisPawnDefinition("pawn_red", playerTemplateId, ParchisBoard.PlayerColor.Red);
        var bluePawnDef = new ParchisPawnDefinition("pawn_blue", playerTemplateId, ParchisBoard.PlayerColor.Blue);
        _catalog.RegisterDefinition(redPawnDef);
        _catalog.RegisterDefinition(bluePawnDef);

        // 2. Prepare Action Command
        var startCommand = new StartGameCommand(_boardFactory, _entityApplierMock.Object);
        var workflowId = startCommand.Action.Id.Value;
        
        // 3. Create initial GameState and Context
        var initialState = GameState.Empty();
        var context = new StartGameActionContext(Guid.NewGuid(), initialState);
        
        // 4. Prepare Mission Data
        var p1 = PlayerId.From("player_1");
        var p2 = PlayerId.From("player_2");
        var playerColors = new Dictionary<PlayerId, ParchisBoard.PlayerColor>
        {
            { p1, ParchisBoard.PlayerColor.Red },
            { p2, ParchisBoard.PlayerColor.Blue }
        };
        var missionData = ParchisMissionFactory.CreateMissionForPlayers(playerColors);
        
        // 5. Board Definition Mock
        var boardDefMock = new Mock<IBoardDefinition>();

        // ACT
        // Start the workflow
        _orchestrator.StartAction(startCommand.Action, context);

        // Step 1: Add Players
        // P1 (Red) - Adds 4 Red Pawns
        var p1Pawns = new List<AgentDeploymentInput> 
        { 
            new AgentDeploymentInput(new AgentDescriptor("pawn_red"), null), 
            new AgentDeploymentInput(new AgentDescriptor("pawn_red"), null),
            new AgentDeploymentInput(new AgentDescriptor("pawn_red"), null),
            new AgentDeploymentInput(new AgentDescriptor("pawn_red"), null)
        };
        _orchestrator.SubmitInput(workflowId.ToString(), new AddPlayerInput(p1, "Red Player", p1Pawns));
        
        // P2 (Blue) - Adds 4 Blue Pawns
        var p2Pawns = new List<AgentDeploymentInput> 
        { 
             new AgentDeploymentInput(new AgentDescriptor("pawn_blue"), null),
             new AgentDeploymentInput(new AgentDescriptor("pawn_blue"), null),
             new AgentDeploymentInput(new AgentDescriptor("pawn_blue"), null),
             new AgentDeploymentInput(new AgentDescriptor("pawn_blue"), null)
        };
        _orchestrator.SubmitInput(workflowId.ToString(), new AddPlayerInput(p2, "Blue Player", p2Pawns));

        // Step 2: Confirm Players
        _orchestrator.SubmitInput(workflowId.ToString(), new ConfirmPlayersInput());

        // Step 3: Select Map (And pass mission with spawn zones)
        _orchestrator.SubmitInput(workflowId.ToString(), new SelectMapInput("parchis_map", boardDefMock.Object, missionData));

        // ASSERT
        // Verify Action Completed
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Completed));

        // Verify Game State
        var resultState = context.State;
        
        Assert.That(resultState, Is.Not.Null, "Action should produce a GameState");
        Assert.That(resultState.Entities.Count, Is.GreaterThanOrEqualTo(8), "Should have deployed at least 8 pawns");
        
        // Create view to check positions
        var view = new GameStateView(resultState, new GameStateOverlay(resultState));

        // Check Red Pawns
        var redEntities = resultState.Entities.Values.Where(e => e.DefinitionId == "pawn_red").ToList();
        Assert.That(redEntities.Count, Is.EqualTo(4), "Should find 4 red pawns");
        foreach(var pawn in redEntities)
        {
            var pos = view.GetPosition(pawn.Id);
            Assert.That(pos, Is.Not.Null, "Red pawn should have a position");
            Assert.That(((TilePosition)pos!).TileId.Value, Is.EqualTo("spawn_red"), "Red pawns should spawn at 'spawn_red'");
        }

        // Check Blue Pawns
        var blueEntities = resultState.Entities.Values.Where(e => e.DefinitionId == "pawn_blue").ToList();
        Assert.That(blueEntities.Count, Is.EqualTo(4), "Should find 4 blue pawns");
        foreach(var pawn in blueEntities)
        {
            var pos = view.GetPosition(pawn.Id);
            Assert.That(pos, Is.Not.Null, "Blue pawn should have a position");
            Assert.That(((TilePosition)pos!).TileId.Value, Is.EqualTo("spawn_blue"), "Blue pawns should spawn at 'spawn_blue'");
        }
    }
}
