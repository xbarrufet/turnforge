using BarelyAlive.Rules.Game;
using BarelyAlive.Rules.Tests.Helpers;
using BarelyAlive.Rules.Tests.Infrastructure;
using NUnit.Framework;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Integration;

[TestFixture]
public class RandomMovementIntegrationTest
{
    [Test]
    public void RandomMovement_SurvivorAndZombie_10RoundsOrMeet()
    {
        // GIVEN: Mission with 1 Survivor and 1 Zombie
        // We use Mission01 which has defined spawn points
        var runner = ScenarioRunner.Create()
            .GivenMission(TestHelpers.Mission01Json)
            .GivenSurvivors("Survivor.Mike")
            .GivenAgents(new SpawnRequest(
                "Zombie.Runner", // DefinitionId
                1, // Count
                new Position(new TileId(Guid.Parse("66a0dadc-d774-4ce9-a3ec-0213e9528af6"))) // Position
            ));
        
        // WHEN: Execute 10 rounds of random movement
        int roundsExecuted = 0;
        bool agentsMet = false;
        const int MaxRounds = 10;
        
        // Initial state check
        Console.WriteLine($"[Test] Initial State: {runner.GetCurrentState().Agents.Count} agents");
        
        while (roundsExecuted < MaxRounds && !agentsMet)
        {
            Console.WriteLine($"[Test] --- Starting Round {roundsExecuted + 1} ---");
            
            // 1. Players Phase
            Console.WriteLine($"[Test] Check FSM State: {runner.GetCurrentState().CurrentStateId}");
            
            var survivorDest = RandomMovementHelper.GetRandomMoveDestination(runner.GetCurrentState(), "Survivor.Mike");
            if (survivorDest != null)
            {
                Console.WriteLine($"[Test] Moving Survivor to {survivorDest}");
                runner.When(cmd => cmd.Move("Survivor.Mike").To(survivorDest.Value));
            }
            else
            {
                Console.WriteLine("[Test] Survivor has no valid moves");
            }
            
            // Loop Survivor Moves until AP exhausted
             while (true)
             {
                 var dest = RandomMovementHelper.GetRandomMoveDestination(runner.GetCurrentState(), "Survivor.Mike");
                 if (dest != null)
                 {
                     Console.WriteLine($"[Test] Moving Survivor to {dest}");
                     runner.When(cmd => cmd.Move("Survivor.Mike").To(dest.Value));
                 }
                 else
                 {
                     break; 
                 }
             }

             // Loop Zombie Moves
             while (true)
             {
                 var dest = RandomMovementHelper.GetRandomMoveDestination(runner.GetCurrentState(), "Zombie.Runner");
                 if (dest != null)
                 {
                     Console.WriteLine($"[Test] Moving Zombie to {dest}");
                     runner.When(cmd => cmd.Move("Zombie.Runner").To(dest.Value));
                 }
                 else
                 {
                     break;
                 }
             }
            
            // Check outcome
            var state = runner.GetCurrentState();
            agentsMet = CheckAgentsMet(state);
            roundsExecuted++;
            
            // Verify Round Counter
            if (state.Metadata.TryGetValue("RoundCounter", out var r))
            {
                Console.WriteLine($"[Test] GameState Round Counter: {r}");
            }
        }
        
        // THEN: Validate end condition
        runner.Then(state =>
        {
            Console.WriteLine($"[Test] Final Rounds: {roundsExecuted}");
            Console.WriteLine($"[Test] Agents Met: {agentsMet}");
            
            Assert.That(roundsExecuted <= MaxRounds, "Should not exceed max rounds");
            Assert.That(agentsMet || roundsExecuted == MaxRounds, "Should end by meeting or 10 rounds");
        });
    }

    private bool CheckAgentsMet(GameState state)
    {
        var survivor = state.GetAgents().FirstOrDefault(a => a.Category == "Survivor");
        var zombie = state.GetAgents().FirstOrDefault(a => a.Category == "Zombie");
        
        if (survivor != null && zombie != null)
        {
            return survivor.PositionComponent.CurrentPosition == zombie.PositionComponent.CurrentPosition;
        }
        return false;
    }
}
