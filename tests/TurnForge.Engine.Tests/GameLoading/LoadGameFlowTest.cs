using NUnit.Framework;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Tests.Bootstrap;

namespace TurnForge.Engine.Tests.GameLoading;

[TestFixture]
public sealed class LoadGameFlowTests
{
    [Test]
    public void LoadGame_creates_game_and_board()
    {
        // Arrange
        var (engine, repository) =
            EngineTestBootstrapper.Boot();

        var mapper = new MissionMapper();
        var dto = MissionLoader.LoadFromFile("Assets/mission01.json");
        var loadGameCommand = mapper.Map(dto);

        // Act
        engine.LoadGame(loadGameCommand);

        // Assert
        var game = repository.GetCurrent();

        Assert.That(game, Is.Not.Null,
            "LoadGame should create and store a Game");

        Assert.That(game!.GameBoard, Is.Not.Null,
            "Game should have a Board");
    }
}