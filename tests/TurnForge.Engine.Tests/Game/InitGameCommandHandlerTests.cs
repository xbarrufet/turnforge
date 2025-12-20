using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Game
{
    public class InitGameCommandHandlerTests
    {
        private class InMemoryRepo : IGameRepository
        {
            public TurnForge.Engine.Entities.GameState State = TurnForge.Engine.Entities.GameState.Empty();
            public TurnForge.Engine.Core.Game? SavedGame;

            public TurnForge.Engine.Entities.GameState LoadGameState() => State;
            public void SaveGameState(TurnForge.Engine.Entities.GameState state) => State = state;
            public void SaveGame(TurnForge.Engine.Core.Game game) => SavedGame = game;
            public TurnForge.Engine.Core.Game LoadGame(GameId gameId) => throw new System.NotImplementedException();
            public TurnForge.Engine.Core.Game? GetCurrent() => SavedGame;
        }

        private class TestEffectSink : IEffectSink
        {
            public readonly List<IGameEffect> Emitted = new();
            public void Emit(IGameEffect effect) => Emitted.Add(effect);
            public void Subscribe(System.Action<IGameEffect> action) { }
        }

        private class TestActorFactory : IActorFactory
        {
            public List<Agent> BuiltAgents = new();
            public Agent BuildAgent(AgentDescriptor descriptor) => Build(descriptor);
            public Prop BuildProp(PropDescriptor descriptor) => Build(descriptor);

            public Agent Build(IGameEntityDescriptor<Agent> descriptor)
            {
                var d = (AgentDescriptor)descriptor;
                var id = EntityId.New();
                var behavioursList = d.ExtraBehaviours?.Cast<IActorBehaviour>().ToList() ?? new List<IActorBehaviour>();
                var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(d.ExtraBehaviours?.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>() ?? Enumerable.Empty<TurnForge.Engine.Entities.Components.BaseBehaviour>());

                var def = new AgentDefinition(d.TypeId,
                    new TurnForge.Engine.Entities.Components.Definitions.PositionComponentDefinition(Position.Empty),
                    new TurnForge.Engine.Entities.Components.Definitions.HealhtComponentDefinition(10),
                    new TurnForge.Engine.Entities.Components.Definitions.MovementComponentDefinition(3),
                    behavioursList);
                var u = new Agent(id, def, new TurnForge.Engine.Entities.Components.PositionComponent(def.PositionComponentDefinition), component);
                BuiltAgents.Add(u);
                return u;
            }

            public Prop Build(IGameEntityDescriptor<Prop> descriptor)
            {
                var d = (PropDescriptor)descriptor;
                var id = EntityId.New();
                var behavioursList = d.ExtraBehaviours?.Cast<IActorBehaviour>().ToList() ?? new List<IActorBehaviour>();
                var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(d.ExtraBehaviours?.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>() ?? Enumerable.Empty<TurnForge.Engine.Entities.Components.BaseBehaviour>());

                var def = new PropDefinition(d.TypeId,
                    new TurnForge.Engine.Entities.Components.Definitions.PositionComponentDefinition(Position.Empty),
                    new TurnForge.Engine.Entities.Components.Definitions.HealhtComponentDefinition(10),
                    behavioursList);
                return new Prop(id, def, new TurnForge.Engine.Entities.Components.PositionComponent(def.PositionComponentDefinition), component);
            }


        }

        private class StubAgentStrategy(IReadOnlyList<AgentSpawnDecision> decisions) : IAgentSpawnStrategy
        {
            public IReadOnlyList<AgentSpawnDecision> Decide(AgentSpawnContext context) => decisions;
        }

        private class StubPropStrategy(IReadOnlyList<PropSpawnDecision> decisions) : IPropSpawnStrategy
        {
            public IReadOnlyList<PropSpawnDecision> Decide(PropSpawnContext context) => decisions;
        }

        private class StubGameFactory : IGameFactory
        {
            public TurnForge.Engine.Core.Game Build(GameBoard board) => new TurnForge.Engine.Core.Game(new GameId(System.Guid.NewGuid()), board);
        }

        private class StubBoardFactory : IBoardFactory
        {
            public GameBoard Build(IGameEntityDescriptor<GameBoard> descriptor) => new GameBoard(new TurnForge.Engine.Spatial.ConnectedGraphSpatialModel(new TurnForge.Engine.Spatial.MutableTileGraph(new HashSet<TileId>())));
        }

        [Test]
        public void Handle_ExecutesAllPhases()
        {
            var repo = new InMemoryRepo();
            var effectSink = new TestEffectSink();
            var factory = new TestActorFactory();
            var gameFactory = new StubGameFactory();
            var boardFactory = new StubBoardFactory();

            // Descriptors
            var t1 = TileId.New();
            var t2 = TileId.New();

            var spatial = new TurnForge.Engine.Commands.LoadGame.Descriptors.DiscreteSpatialDescriptor(
                new List<TileId> { t1, t2 },
                new List<TurnForge.Engine.Commands.LoadGame.Descriptors.DiscreteConnectionDeacriptor>()
            );
            var zones = new List<ZoneDescriptor>();
            var props = new List<PropDescriptor>();
            var unit1 = new AgentDescriptor(new AgentTypeId("u1"), new Position(t2));

            // Decisions
            var agentDecision = new AgentSpawnDecision(unit1.TypeId, new Position(t2), new List<IActorBehaviour>());
            var agentStrategy = new StubAgentStrategy(new[] { agentDecision });
            var propStrategy = new StubPropStrategy(new List<PropSpawnDecision>());

            var handler = new InitGameCommandHandler(factory, gameFactory, repo, boardFactory, propStrategy, agentStrategy, effectSink);

            var command = new InitGameCommand(
                Spatial: spatial,
                Zones: zones,
                StartingProps: props,
                Agents: new List<AgentDescriptor> { unit1 }
            );
            var result = handler.Handle(command);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Tags, Contains.Item("GameInitialized"));
            Assert.That(result.Tags, Contains.Item("StartFSM"));

            // Verify Game Saved
            // Verify Game Saved - Obsolete: Handler returns decisions now.
            // Assert.That(repo.SavedGame, Is.Not.Null);

            // Verify Decisions returned (currently empty prop strategy, so empty)
            Assert.That(result.Decisions, Is.Not.Null);
        }
    }
}
