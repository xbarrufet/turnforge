using NUnit.Framework;
using BarelyAlive.Rules.Tests.Infrastructure;
using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.ValueObjects;
using System.Linq;
using TurnForge.Engine.Commands.ACK;
using BarelyAlive.Rules.Core.Domain.Entities;

namespace BarelyAlive.Rules.Tests.Game;

[TestFixture]
public class TestInitGame
{
    [Test]
    public void InitGame_ShouldUseSpawnStrategies_Correctly()
    {
        // 1. Create Game with Custom Strategies
        // 1. Create Game with Custom Strategy to verify injection
        var bootstrap = TestBootstrap.CreateNewGame(
            agentStrategy: new BarelyAlive.Rules.Tests.Infrastructure.Strategies.TestAgentSpawnStrategy()
        );
        
        // 2. Parse Mission
        var (spatial, zones, props, agents) = BarelyAlive.Rules.Adapter.Loaders.MissionLoader.ParseMissionString(TestHelpers.Mission01Json);
        var boardDesc = new TurnForge.Engine.Entities.Board.Descriptors.BoardDescriptor(spatial, zones);
        
        // 3. Execute Commands (Mimic InitializeGameHandler + StartGame)
        
        // 3.1 Init Board
        var res1 = bootstrap.Engine.Runtime.ExecuteCommand(new InitializeBoardCommand(boardDesc));
        Assert.That(res1.Result.Success, Is.True, $"InitBoard failed: {res1.Result.Error}");
        
        var ack1 = bootstrap.Engine.Runtime.ExecuteCommand(new ACKCommand());
        


        // 3.2 Spawn Props (Triggers Prop Strategy)
        var res2 = bootstrap.Engine.Runtime.ExecuteCommand(new SpawnPropsCommand(props));
        Assert.That(res2.Result.Success, Is.True, $"SpawnProps failed: {res2.Result.Error}");
        
        var ack2 = bootstrap.Engine.Runtime.ExecuteCommand(new ACKCommand());

        // 3.3 Spawn Agents (Triggers Agent Strategy)
        // Manually create agents to test Strategy logic (Mission01Json doesn't have agents)
        // We give one of them a dummy position (0,0), hoping the Strategy moves them to Spawn.Player (1,1)
        var dummyPos = new Position(new TileId(System.Guid.Parse("d7de841d-64a5-48b3-9662-0fe757a8950e"))); // (0,0)
        var testAgents = new List<SpawnRequest>
        {
             new SpawnRequest(
                 DefinitionId: TestHelpers.MikeId, 
                 TraitsToOverride: new List<TurnForge.Engine.Traits.Interfaces.IBaseTrait> { new TurnForge.Engine.Traits.Standard.PositionTrait(dummyPos) }
             ),
             new SpawnRequest(TestHelpers.DougId)
        };
        
        var res3 = bootstrap.Engine.Runtime.ExecuteCommand(new SpawnAgentsCommand(testAgents));
        Assert.That(res3.Result.Success, Is.True, $"SpawnAgents failed: {res3.Result.Error}");
        
        // 4. Verification
        var state = bootstrap.GameRepository.LoadGameState();
        var propsInGame = state.GetProps();
        
        System.Console.WriteLine($"[TEST DEBUG] Props Count: {propsInGame.Count}");
        foreach(var p in propsInGame)
        {
             System.Console.WriteLine($"   - Prop {p.Id}: DefId='{p.DefinitionId}' Pos={p.PositionComponent?.CurrentPosition}");
        }
        var agentsInGame = state.GetAgents();
        
        // check that the agents have been created
        System.Console.WriteLine($"[TEST DEBUG] Agents Count: {agentsInGame.Count}");
        Assert.That(agentsInGame.Count, Is.EqualTo(2));
        
        // Check that they are created in the Spawn.Player position (1,1)
        // ID: 07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53 (from Mission01Json)
        var expectedSpawnPos = new Position(new TileId(System.Guid.Parse("07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53")));
        
        foreach(var agent in agentsInGame)
        {
            System.Console.WriteLine($"[TEST DEBUG] Agent {agent.Id} at {agent.PositionComponent.CurrentPosition}. Expected: {expectedSpawnPos}");
            Assert.That(agent.PositionComponent.CurrentPosition, Is.EqualTo(expectedSpawnPos), $"Agent {agent.Id} should be at PlayerSpawn position {expectedSpawnPos}");
            Assert.That(agent.PositionComponent.CurrentPosition, Is.Not.EqualTo(dummyPos), "Agent should have been moved from dummy position");
        }
        
        // Also verify Props count
        Assert.That(propsInGame.Count, Is.EqualTo(12));
    }
}