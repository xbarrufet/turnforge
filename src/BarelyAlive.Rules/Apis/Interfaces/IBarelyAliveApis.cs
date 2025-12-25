using BarelyAlive.Rules.Apis.Messaging;

namespace BarelyAlive.Rules.Apis.Interfaces;

public interface IBarelyAliveApis
{
    GameResponse InitializeGame(string missionJson);
    GameResponse StartGame(string[] survivorIds);
    List<SurvivorDefinition> GetAvailableSurvivors();
    void Ack();
    
    // Movement and Query APIs
    GameResponse MoveCharacter(string characterId, TileReference targetTile);
    GameStateSnapshot GetGameState();
}