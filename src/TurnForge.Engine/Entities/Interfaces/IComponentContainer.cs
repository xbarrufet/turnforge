using System.Collections.Generic;
using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Entities.Interfaces;

public interface IComponentContainer
{
    IEnumerable<IGameEntityComponent> GetAllComponents();
}
