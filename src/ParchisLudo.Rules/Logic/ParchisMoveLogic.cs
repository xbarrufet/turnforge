using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions; // For GameEntity
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board; // For ConnectionEntity if needed
using TurnForge.Engine.ValueObjects; // For TilePosition, TileId

namespace Parchis.Rules.Logic;

/// <summary>
/// Core movement logic for Parchís.
/// Handles path traversal, color restrictions, center completion, and bounce-back rules.
/// </summary>
public static class ParchisMoveLogic
{
    /// <summary>
    /// Calculates the destination tile for a pawn moving N steps.
    /// Handles forward movement, entering finish lane, stopping at center, and bouncing back.
    /// </summary>
    public static TileId? CalculateDestination(
        GameStateView state,
        TileId startTile,
        int steps,
        string teamColor,
        out bool reachedCenter,
        out bool bounced)
    {
        reachedCenter = false;
        bounced = false;
        
        var currentTile = startTile;
        var remainingSteps = steps;
        
        // 1. Forward Movement
        while (remainingSteps > 0)
        {
            // Get valid connections from current tile
            var connections = state.GetConnectionsForTeam(currentTile, teamColor).ToList();
            
            if (connections.Count == 0)
            {
                // Dead end (should not happen in Parchís unless map is broken)
                return null;
            }
            
            // Prefer "finish_entry" if available and restricted to our team, otherwise generic "forward"
            GameEntity? selectedConnection = null;
            
            // Priority 1: Finish Entry (entering home stretch)
            selectedConnection = connections.FirstOrDefault(c => c.Category == "finish_entry");
            
            // Priority 2: Finish Complete (entering center)
            if (selectedConnection == null)
                selectedConnection = connections.FirstOrDefault(c => c.Category == "finish_complete");
                
            // Priority 3: Forward (standard track)
            if (selectedConnection == null)
                selectedConnection = connections.FirstOrDefault(c => c.Category == "forward");
            
            if (selectedConnection == null) return null; // No valid path
            
            // Move to next tile
            var pos = selectedConnection.GetComponent<IPositionComponent>()?.CurrentPosition as ConnectionPosition?;
            if (pos == null) return null;
            
            currentTile = pos.Value.To;
            remainingSteps--;
            
            // Check if reached Center
            if (currentTile.Value == "center")
            {
                if (remainingSteps == 0)
                {
                    reachedCenter = true;
                    return currentTile;
                }
                else
                {
                    // Overshot center - Bounce back logic starts here
                    bounced = true;
                    var previousTile = pos.Value.From;
                    
                    // One step is consumed to go from Center back to the previous tile
                    // So we bounce back 'remainingSteps - 1' from previousTile
                    // If remainingSteps was 1, we land on previousTile (BounceBack(..., 0) -> startBounceTile)
                    return BounceBack(state, previousTile, remainingSteps - 1, teamColor);
                }
            }
        }
        
        return currentTile;
    }
    
    private static TileId? BounceBack(GameStateView state, TileId startBounceTile, int steps, string teamColor)
    {
        var current = startBounceTile;
        
        for (int i = 0; i < steps; i++)
        {
            // Find backward path.
            // Hack for now: Parse ID "red_finish_7" -> "red_finish_6"
            if (current.Value.Contains("_finish_"))
            {
                var parts = current.Value.Split('_'); // [red, finish, 7]
                if (parts.Length == 3 && int.TryParse(parts[2], out int index))
                {
                    if (index > 1) 
                    {
                        current = new TileId($"{parts[0]}_{parts[1]}_{index - 1}");
                    }
                    else 
                    {
                        // Assume clamping at finish_1
                        current = new TileId($"{parts[0]}_{parts[1]}_1");
                    }
                }
            }
        }
        return current;
    }
}
