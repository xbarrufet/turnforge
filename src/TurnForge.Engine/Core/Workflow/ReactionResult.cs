
using System;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow
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
        public WorkflowContext Context { get; }

        /// <summary>
        /// Optional modified input produced by the reaction.
        /// Used for rerolls, replacements or adjustments.
        /// </summary>
        public IWorkflowInput? ModifiedInput { get; }

        /// <summary>
        /// Optional nested workflow to be executed immediately.
        /// Nested workflows are fully resolved before continuing.
        /// </summary>
        public IWorkflow? NestedWorkflow { get; }

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
        /// If false but NestedWorkflow is set, it might indicate a potential workflow that IS NOT YET triggered (e.g. requires input).
        /// </summary>
        public bool ExecuteNestedWorkflow { get; }

        /// <summary>
        /// Indicates whether the reaction caused any change.
        /// Useful for orchestrator optimizations and tracing.
        /// </summary>
        public bool HasEffect =>
            ModifiedInput is not null || NestedWorkflow is not null || RequiresInput;

        private ReactionResult(
            WorkflowContext context,
            IWorkflowInput? modifiedInput,
            IWorkflow? nestedWorkflow,
            bool requiresInput = false,
            bool executeNestedWorkflow = false)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ModifiedInput = modifiedInput;
            NestedWorkflow = nestedWorkflow;
            RequiresInput = requiresInput;
            ExecuteNestedWorkflow = executeNestedWorkflow;
        }

        // --------------------------------------------------------------------
        // Factory methods
        // --------------------------------------------------------------------

        /// <summary>
        /// The reaction had no effect.
        /// The workflow continues unchanged.
        /// </summary>
        public static ReactionResult NoChange(WorkflowContext context)
            => new(context, null, null);

        /// <summary>
        /// The reaction modified the current input.
        /// </summary>
        public static ReactionResult WithModifiedInput(
            WorkflowContext context,
            IWorkflowInput modifiedInput)
            => new(context, modifiedInput, null);

        /// <summary>
        /// The reaction requires external input.
        /// </summary>
        public static ReactionResult InputRequired(WorkflowContext context)
            => new(context, null, null, requiresInput: true);

        /// <summary>
        /// The reaction launches a nested workflow.
        /// </summary>
        public static ReactionResult WithNestedWorkflow(
            WorkflowContext context,
            IWorkflow nestedWorkflow,
            bool executeImmediately = true)
            => new(context, null, nestedWorkflow, executeNestedWorkflow: executeImmediately);

        /// <summary>
        /// The reaction both modifies input and launches a nested workflow.
        /// This is rare but allowed.
        /// </summary>
        public static ReactionResult WithModifiedInputAndNestedWorkflow(
            WorkflowContext context,
            IWorkflowInput modifiedInput,
            IWorkflow nestedWorkflow,
            bool executeImmediately = true)
            => new(context, modifiedInput, nestedWorkflow, executeNestedWorkflow: executeImmediately);
    }
}

