using TurnForge.Engine.Commands.StartGame.Action;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Spawn;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame;

/// <summary>
/// Final node in StartGame workflow that builds the initial game state.
/// Uses InitialGameStateBuilder to construct complete state from pending deployments.
/// Replaces the overlay-based spawn approach with direct state construction.
/// </summary>
public class BuildInitialStateNode : LinkableNode
{
    private readonly ISpawnService _spawnService;

    public override NodeId Id { get; }

    public BuildInitialStateNode(NodeId id, ISpawnService spawnService)
    {
        Id = id;
        _spawnService = spawnService;
    }

    
    
    
    public override ActionStepResult Execute(ActionContext context, GameStateView gameStateView)
    {
       throw new NotImplementedException();
        /*  // Use typed context for all operations
          if (context is not StartGameActionContext typedContext)
          {
              throw new InvalidOperationException("BuildInitialStateNode requires StartGameActionContext");
          }

          // Get current board from base state (created by ProcessBoardDataNode via overlay)
          var board = typedContext.GameBoard;
          if (board == null)
          {
              throw new InvalidOperationException("Board must be created before building initial state");
          }

          // Get players from base state (created by ProcessPlayerDataNode via overlay)
          var players = typedContext.Players;
          if (players.Count == 0)
          {
              throw new InvalidOperationException("At least one player must be added before building initial state");
          }

          // Create builder
          var builder = GameState.CreateBuilder()
              .WithPlayers(players)
              .WithGameBoard(board);

          // Add mission if provided
          if (typedContext.MissionData != null)
          {
              // TODO: Load mission from MissionDataInput
              // builder.WithMission(mission);
          }

          // Add agents with positions
          foreach (var agentDeploy in typedContext.PendingAgentDeployments)
          {
              var position = agentDeploy.Position ?? IBoardPosition.Limbo;
              var entity = CreateAgent(agentDeploy);
              builder.WithStartingAgent(entity, position);
          }

          // Add zones with positions
          foreach (var zoneDeploy in typedContext.PendingZoneDeployments)
          {
              var entity = CreateZone(zoneDeploy);
              builder.WithStartingZone(entity, zoneDeploy.Position);
          }

          // Add connections with positions
          foreach (var connDeploy in typedContext.PendingConnectionDeployments)
          {
              var entity = CreateConnection(connDeploy);
              builder.WithConnection(entity, connDeploy.Position);
          }

          // Add props with positions
          foreach (var propDeploy in typedContext.PendingPropDeployments)
          {
              var entity = CreateProp(propDeploy);
              builder.WithStartingProp(entity, propDeploy.Position);
          }

          // Build initial state
          var initialState = builder.Build();

          // Replace entire state in context
          // Note: This is a special operation for StartGame only
          // Runtime entity creation should use ISpawnService with overlay
           gameStateView.ResetState(initialState);

          return ActionStepResult.Success();
      }

      private GameEntity CreateAgent(AgentDeployment deployment)
      {
          // Use spawn service to create entity from descriptor
          // Descriptor = DefinitionId + ExtraComponents + RequestedTraits
          var spawnOp = _spawnService.Spawn(deployment.Descriptor, deployment.Position);
          return _spawnService.PositionActor((Actor)spawnOp.Entity, deployment.Position);


      }

      private GameEntity CreateZone(ZoneDeployment deployment)
      {
          // Zone uses Descriptor (not Definition)
          var spawnOp = _spawnService.Spawn(deployment.Zone, deployment.Position);
          return spawnOp.Entity;
      }

      private GameEntity CreateConnection(ConnectionDeployment deployment)
      {
          // Use spawn service to create entity from descriptor
          // Descriptor = DefinitionId + ExtraComponents + RequestedTraits
          var spawnOp = _spawnService.Spawn(deployment.Descriptor, deployment.Position);
          return spawnOp.Entity;
      }

      private GameEntity CreateProp(PropDeployment deployment)
      {
          // Prop uses Definition (legacy, will be migrated to Descriptor)
          var spawnOp = _spawnService.Spawn(deployment.Definition, deployment.Position);
          return _spawnService.PositionActor((Actor)spawnOp.Entity, deployment.Position);*/
    }
}
