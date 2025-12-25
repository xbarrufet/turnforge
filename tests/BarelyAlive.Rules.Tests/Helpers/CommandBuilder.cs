using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Helpers;

/// <summary>
/// Fluent builder for creating TurnForge commands in tests.
/// Provides a clean, readable API for command construction.
/// </summary>
public class CommandBuilder
{
    /// <summary>
    /// Creates a move command builder for the specified agent.
    /// </summary>
    /// <param name="agentId">The ID of the agent to move</param>
    /// <returns>A move command builder</returns>
    public static MoveCommandBuilder Move(string agentId)
    {
        return new MoveCommandBuilder(agentId);
    }

    /// <summary>
    /// Builder for move commands.
    /// </summary>
    public class MoveCommandBuilder
    {
        private readonly string _agentId;

        internal MoveCommandBuilder(string agentId)
        {
            if (string.IsNullOrWhiteSpace(agentId))
            {
                throw new ArgumentException("Agent ID cannot be null or empty", nameof(agentId));
            }
            
            _agentId = agentId;
        }

        /// <summary>
        /// Specifies the target position for the move command.
        /// </summary>
        /// <param name="target">The target position</param>
        /// <returns>The completed move command</returns>
        public MoveCommand To(Position target)
        {
            if (target == Position.Empty)
            {
                throw new ArgumentException("Target position cannot be empty", nameof(target));
            }

            return new MoveCommand(_agentId, hasCost: true, targetPosition: target);
        }

        /// <summary>
        /// Specifies the target position by TileId for the move command.
        /// </summary>
        /// <param name="tileId">The target tile ID</param>
        /// <returns>The completed move command</returns>
        public MoveCommand To(TileId tileId)
        {
            return To(new Position(tileId));
        }

        /// <summary>
        /// Specifies the target position by Guid for the move command.
        /// </summary>
        /// <param name="tileGuid">The target tile GUID</param>
        /// <returns>The completed move command</returns>
        public MoveCommand To(Guid tileGuid)
        {
            return To(new TileId(tileGuid));
        }
    }
}
