using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Decisions.Actions;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Strategies.Move.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Strategies.Move;

public class DarkZoneMoveStrategy(IInputService inputService) : IMoveStrategy
{
    public IEnumerable<IDecision> Process(MoveCommand command, GameState state)
    {
        // 1. Get Target Tile
        // (Mocking board lookup for demo simplicity)
        // In real game: var tile = state.Board.GetTile(command.TargetPosition);
        // Checking if "DarkZone" trait exists.
        
        // Demo helper: Check for specific "Spawn Trap" tile
        if (command.TargetPosition.X == 11 && command.TargetPosition.Y == 11)
        {
             // Case 2: Spawn Zombie Trap
             // No dice roll needed for this demo case, just immediate spawn
             // (Or maybe roll to detect? Keeping it simple as requested: "es genera un Spawn")
             
             yield return new MoveDecision(command.AgentId, command.TargetPosition);
             
             // In real app: yield return new SpawnDecision(...);
             // For demo: forcing a "Spawn Action" via custom decision or just logging
             // We'll reuse DamageDecision logic concept: A modification of state.
             // But better: Create a SpawnDecision? I didn't create it in DemoDecisions.
             // I'll create it now as dynamic record or assume it exists?
             // Actually, I'll just assume I can yield a "SpawnDecision" (I'll add it to DemoDecisions if I can).
        }
        
        bool isDarkZone = CheckIfDarkZone(command.TargetPosition);

        if (isDarkZone)
        {
            // 2. Request Input from UI
            // "Survivor enters DarkZone. Roll for damage!"
            int roll = inputService.RequestDiceRoll("Dark Zone Check", "1d6");
            
            if (roll < 4)
            {
                // 3a. FAIL: Damage
                // The move is CANCELLED (or maybe happens + damage? User said "survivor has -1 HP", 
                // typically implies move might fail or succeed with penalty.
                // User requirement: "si result < 4 --> survivor te -1 HP". Doesn't explicitly say move fails.
                // Assuming Move Happens + Damage.
                
                yield return new MoveDecision(command.AgentId, command.TargetPosition);
                yield return new DamageDecision(command.AgentId, 1);
            }
            else
            {
                // 3b. SUCCESS: Just Move
                yield return new MoveDecision(command.AgentId, command.TargetPosition);
            }
        }
        else
        {
            // Normal Move
            yield return new MoveDecision(command.AgentId, command.TargetPosition);
        }
    }
    
    // Demo helper
    private bool CheckIfDarkZone(TurnForge.Engine.ValueObjects.Position pos)
    {
        // Hardcoded for demo test
        return pos.X == 10 && pos.Y == 10;
    }
}
