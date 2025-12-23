using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Core.Fsm;

public enum PhaseResult
{
    Pass,
    LaunchCommand,
    ApplyDecisions
}

public struct NodePhaseResult
{
    public PhaseResult Result { get; set; }
    public ICommand? Command { get; set; }
    public IEnumerable<IFsmApplier>? Decisions { get; set; }

    public static NodePhaseResult Pass() => new() { Result = PhaseResult.Pass };
    
    public static NodePhaseResult Launch(ICommand command, IEnumerable<IFsmApplier>? decisions = null) => 
        new() { Result = PhaseResult.LaunchCommand, Command = command, Decisions = decisions };
    
    public static NodePhaseResult Apply(IEnumerable<IFsmApplier> decisions) => 
        new() { Result = PhaseResult.ApplyDecisions, Decisions = decisions };
}
