using System.Collections.Generic;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;

namespace TurnForge.Engine.Definitions.Interfaces;

public interface IComponentContainer
{
    IEnumerable<IGameEntityComponent> GetAllComponents();
    IEnumerable<ITrait> GetAllTraits();
}
