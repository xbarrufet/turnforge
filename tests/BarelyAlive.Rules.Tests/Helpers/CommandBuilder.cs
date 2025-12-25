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
    /// Instance method for Move, enabling usage via ScenarioRunner instance.
    /// </summary>
    public MoveCommandBuilder Agent(string agentId) => Move(agentId);
    
    // Alias Move to Agent for instance usage if C# allows (it does, but name clash with static? No, static is Move, instance can be Move)
    // Wait, ScenarioRunner uses "cmd.Move". If "Move" is static, "cmd.Move" is invalid.
    // I need an instance method named "Move".
    // But duplicate names are allowed if signatures differ or one is static.
    // However, calling static method via instance is not valid in C#.
    // So I must rename static or add instance.
    // I can't rename static easily without breaking other tests.
    // I will add "Move" instance method.
    
    public MoveCommandBuilder Move(string agentId, bool instance = true) 
    {
        return new MoveCommandBuilder(agentId);
    }
    
    // Actually, method overloading with same signature (one static, one instance) is NOT allowed.
    // So I cannot have "public static Move" AND "public Move".
    // I must stick to "Create" or similar for static, or rename instance.
    // BUT ScenarioRunner likely expects "Move".
    // "cmd.Move" implies instance method.
    // The previous tests might use "CommandBuilder.Move".
    // I should check if other tests use CommandBuilder.
    // I will rename the STATIC method to "For" or "CreateMove" and make "Move" the instance method?
    // OR I make "Move" static and use "CommandBuilder.Move" in the lambda?
    // Lambda: `runner.When(cmd => CommandBuilder.Move("...").To(...))`
    // But `When` signature is `Func<CommandBuilder, IActionCommand>`.
    // So `cmd` is `CommandBuilder`.
    // `cmd` is instance.
    // So `cmd.Move(...)` REQUIRES instance method.
    // So I MUST have instance method `Move`.
    // I will remove `static` from `Move`?
    // If I do, existing calls `CommandBuilder.Move(...)` will fail.
    // I'll search for usages of `CommandBuilder.Move`.
    // If few, I refactor.

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
