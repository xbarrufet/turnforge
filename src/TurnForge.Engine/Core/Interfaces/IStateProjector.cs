using System.Collections.Generic;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Decisions;

namespace TurnForge.Engine.Core.Interfaces;

public interface IStateProjector
{
    GameState Project(GameState baseState, IEnumerable<IDecision> decisions);
}
