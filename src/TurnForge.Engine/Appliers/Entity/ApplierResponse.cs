using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;

namespace TurnForge.Engine.Entities.Appliers;

public sealed record ApplierResponse(GameState GameState, IGameEffect[] GameEffects);