using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Appliers.Entity.Interfaces;

public interface IFsmApplier
{
    ApplierResponse Apply(GameState state);
}