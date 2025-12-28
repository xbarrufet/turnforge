using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Core.Workflow.Interfaces;

 public interface IProducesDecisions
    {
        IReadOnlyList<IDecision> BuildDecisions(
            WorkflowContext context);
    }