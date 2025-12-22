using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Game;

namespace BarelyAlive.Godot.Adapter.Adapters;

public class QueryCatalogAdapter
{
    private readonly BarelyAliveGame _game;

    public QueryCatalogAdapter(BarelyAliveGame game)
    {
        _game = game;
    }

    public List<SurvivorDefinition> GetSurvivorsFromCatalog()
    {
        return _game.BarelyAliveApis.GetAvailableSurvivors();
    }
}