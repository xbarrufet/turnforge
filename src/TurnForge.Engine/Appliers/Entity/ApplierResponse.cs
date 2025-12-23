using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace TurnForge.Engine.Appliers.Entity;

public sealed record ApplierResponse(GameState GameState, IGameEffect[] GameEffects);