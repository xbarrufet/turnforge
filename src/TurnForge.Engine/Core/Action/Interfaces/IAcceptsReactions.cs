namespace TurnForge.Engine.Core.Workflow.Interfaces;

 public interface IAcceptsReactions
    {
        IReadOnlyCollection<IReaction> AllowedReactions { get; }
    }
