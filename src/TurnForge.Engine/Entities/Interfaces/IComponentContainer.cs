using System.Collections.Generic;
using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Definitions.Interfaces;

public interface IComponentContainer
{
    IEnumerable<IGameEntityComponent> GetAllComponents();
}
