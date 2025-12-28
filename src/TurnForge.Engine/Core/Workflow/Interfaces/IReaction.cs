using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow.Interfaces
{
    /// <summary>
    /// A Reaction represents a game rule that may respond
    /// to the current workflow context.
    ///
    /// Reactions:
    /// - NEVER initiate a root workflow
    /// - NEVER mutate persistent game state
    /// - MAY require external input
    /// - MAY modify the workflow context
    /// - MAY modify the current input
    /// - MAY launch a nested workflow
    ///
    /// All game rules live in Reactions.
    /// </summary>
    public interface IReaction
    {
        /// <summary>
        /// Unique identifier of the reaction.
        /// Used for tracing, debugging and rule management.
        /// </summary>
        ReactionId Id { get; }

        /// <summary>
        /// Determines whether this reaction is applicable
        /// given the current workflow context.
        ///
        /// This method MUST be:
        /// - side-effect free
        /// - deterministic
        /// </summary>
        bool CanReact(WorkflowContext context);

        /// <summary>
        /// Executes the reaction.
        ///
        /// The input parameter MAY be null, depending on:
        /// - the current node
        /// - whether input has already been provided
        ///
        /// The reaction MUST NOT:
        /// - mutate persistent game state
        /// - advance the workflow directly
        ///
        /// All consequences are expressed via ReactionResult.
        /// </summary>
        ReactionResult React(
            WorkflowContext context,
            IInputActionResult? input);
    }
}
