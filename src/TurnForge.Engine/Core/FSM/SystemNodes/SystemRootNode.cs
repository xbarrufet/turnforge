
using TurnForge.Engine.Core.Fsm.Interfaces;

namespace TurnForge.Engine.Core.Fsm.SystemNodes
{
    public class SystemRootNode : BranchNode
    {
        public override IReadOnlyList<Type> GetAllowedCommands()
        {
            return [];
        }
    }
}
