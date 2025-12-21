using BarelyAlive.Rules.Apis.Messaging;

namespace BarelyAlive.Rules.Apis.Interfaces;

public interface IBarelyAliveApis
{

    GameResponse InitializeGame(String missionJson);
    GameResponse StartGame();

}