using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Services.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class TurnForge
{
    public IGameEngine Runtime { get; }
    public IGameCatalogApi GameCatalog { get; }

    internal TurnForge(
        IGameEngine runtime,
        IGameCatalogApi gameCatalog)
    {
        Runtime = runtime;
        GameCatalog = gameCatalog;
    }
}
