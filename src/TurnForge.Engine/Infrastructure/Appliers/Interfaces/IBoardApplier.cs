using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Entities.Board;

namespace TurnForge.Engine.Infrastructure.Appliers;

public interface IBoardApplier
{
    GameBoard Apply(InitializeGameCommand command);
}