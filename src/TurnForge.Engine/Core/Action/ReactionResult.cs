
using System;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action
{
    /// <summary>
    /// Represents the outcome of executing a Reaction.
    ///
    /// A ReactionResult does NOT apply effects directly.
    /// It only describes what should happen next inside the workflow.
    /// </summary>
    public sealed class ReactionResult
    {
        /// <summary>
        /// Updated workflow context after the reaction.
        /// The context is mutable, but the reference is explicit.
        /// </summary>
        public ActionContext Context { get; }

        /// <summary>
        /// Optional modified input produced by the reaction.
        /// Used for rerolls, replacements or adjustments.
        /// </summary>
        public IActionInput? ModifiedInput { get; }

        /// <summary>
        /// Optional nested workflow to be executed immediately.
        /// Nested workflows are fully resolved before continuing.
        /// </summary>
        public IAction? NestedAction { get; }

        /// <summary>
        /// Indicates whether the reaction caused any change.
        /// Useful for orchestrator optimizations and tracing.
        /// </summary>
        /// <summary>
        /// Indicates if the reaction requires external input to proceed.
        /// If this is true, the workflow should suspend.
        /// </summary>
        public bool RequiresInput { get; }

        /// <summary>
        /// Indicates whether the reaction caused any change.
        /// Useful for orchestrator optimizations and tracing.
        /// </summary>
        /// <summary>
        /// Indicates if the nested workflow should be executed.
        /// If false but NestedAction is set, it might indicate a potential workflow that IS NOT YET triggered (e.g. requires input).
        /// </summary>
        public bool ExecuteNestedAction { get; }

        /// <summary>
        /// Indicates whether the reaction caused any change.
        /// Useful for orchestrator optimizations and tracing.
        /// </summary>
        public bool HasEffect =>
            ModifiedInput is not null || NestedAction is not null || RequiresInput;

        private ReactionResult(
            ActionContext context,
            IActionInput? modifiedInput,
            IAction? nestedAction,
            bool requiresInput = false,
            bool executeNestedAction = false)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ModifiedInput = modifiedInput;
            NestedAction = nestedAction;
            RequiresInput = requiresInput;
            ExecuteNestedAction = executeNestedAction;
        }

        // --------------------------------------------------------------------
        // Factory methods
        // --------------------------------------------------------------------

        /// <summary>
        /// The reaction had no effect.
        /// The workflow continues unchanged.
        /// </summary>
        public static ReactionResult NoChange(ActionContext context)
            => new(context, null, null);

        /// <summary>
        /// The reaction modified the current input.
        /// </summary>
        public static ReactionResult WithModifiedInput(
            ActionContext context,
            IActionInput modifiedInput)
            => new(context, modifiedInput, null);

        /// <summary>
        /// The reaction requires external input.
        /// </summary>
        public static ReactionResult InputRequired(ActionContext context)
            => new(context, null, null, requiresInput: true);

        /// <summary>
        /// The reaction launches a nested workflow.
        /// </summary>
        public static ReactionResult WithNestedAction(
            ActionContext context,
            IAction nestedAction,
            bool executeImmediately = true)
            => new(context, null, nestedAction, executeNestedAction: executeImmediately);

        /// <summary>
        /// The reaction both modifies input and launches a nested workflow.
        /// This is rare but allowed.
        /// </summary>
        public static ReactionResult WithModifiedInputAndNestedAction(
            ActionContext context,
            IActionInput modifiedInput,
            IAction nestedAction,
            bool executeImmediately = true)
            => new(context, modifiedInput, nestedAction, executeNestedAction: executeImmediately);
    }
}

