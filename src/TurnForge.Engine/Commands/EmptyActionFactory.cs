using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands;

public class EmptyActionFactory:IActionFactory
{
    public IAction BuildAction(ActionId actionId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<ActionId> GetRegisteredActionIds()
    {
        return new List<ActionId>();
    }
}