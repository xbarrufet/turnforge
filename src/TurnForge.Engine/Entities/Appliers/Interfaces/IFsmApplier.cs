using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IFsmApplier
{
    ApplierResponse Apply(GameState state);
}