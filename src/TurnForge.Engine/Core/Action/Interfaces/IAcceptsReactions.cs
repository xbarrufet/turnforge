namespace TurnForge.Engine.Core.Action.Interfaces;

 public interface IAcceptsReactions
    {
        IReadOnlyCollection<IReaction> AllowedReactions { get; }
    }
